using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TFA.Application.Interfaces.Repositories;
using TFA.Client.Config;
namespace TFA.Client.Data.Repositories;

public sealed class ReadOnlyFirebaseRepository : IReadOnlyFirebaseRepository
{
    private readonly ILogger<ReadOnlyFirebaseRepository> _logger;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;
    private readonly FirebaseOptions _firebaseOptions;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ReadOnlyFirebaseRepository(
        ILogger<ReadOnlyFirebaseRepository> logger,
        IMemoryCache cache,
        IOptions<FirebaseOptions> firebaseOptions,
        HttpClient httpClient)
    {
        _logger = logger;
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(15))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        _firebaseOptions = firebaseOptions.Value;
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async ValueTask<TData> Get<TData>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out TData? cachedData) && cachedData is not null)
        {
            return cachedData;
        }
        else if (await GetData<TData>(key, cancellationToken) is TData dbData)
        {
            _cache.Set(key, dbData, _cacheOptions);
            return dbData;
        }
        else
        {
            throw new InvalidDataException(typeof(TData).Name);
        }
    }

    private async Task<TData?> GetData<TData>(string key, CancellationToken cancellationToken)
    {
        string? authToken = await Authenticate();
        string url = $"{_firebaseOptions.Url}/{key}.json?auth={authToken}";

        if (await _httpClient.GetAsync(url, cancellationToken) is { IsSuccessStatusCode: true } response)
        {
            string? body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (body is null || body == "null")
                return default;

            FirebaseResponse dbResult = JsonSerializer.Deserialize<FirebaseResponse>(body);
            if (dbResult is { Data.Count: > 0 })
            {
                return ((JsonElement)dbResult.Data.Values.First()).Deserialize<TData>(_jsonOptions);
            }
        }
        else
        {
            _logger.LogError("Failed to get data from database with key {Key}", key);
        }

        return default;

    }

    private async Task<string> Authenticate()
    {
        FirebaseAuthClient authClient = new(new FirebaseAuthConfig
        {
            ApiKey = _firebaseOptions.ApiKey,
            AuthDomain = _firebaseOptions.AuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        });

        UserCredential authResult = await authClient
            .SignInWithEmailAndPasswordAsync(_firebaseOptions.Username, _firebaseOptions.Password);

        return authResult is { User.Credential.IdToken: not null }
            ? authResult.User.Credential.IdToken
            : throw new FirebaseAuthException("Failed to log in to database.", AuthErrorReason.UserNotFound);
    }
}