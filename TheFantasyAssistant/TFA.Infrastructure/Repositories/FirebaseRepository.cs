using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TFA.Application.Common.Extensions;
using TFA.Application.Interfaces.Repositories;

namespace TFA.Infrastructure.Repositories;

public class FirebaseRepository : IFirebaseRepository
{
    private readonly ILogger<FirebaseRepository> _logger;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;
    private readonly IHttpClientFactory _clientFactory;
    private readonly FirebaseOptions _firebaseOptions;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ISourceFetcherService _sourceFetcher;

    public FirebaseRepository(
        ILogger<FirebaseRepository> logger, 
        IMemoryCache cache, 
        IHttpClientFactory clientFactory, 
        IOptions<FirebaseOptions> firebaseOptions,
        ISourceFetcherService sourceFetcher)
    {
        _logger = logger;
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
        _clientFactory = clientFactory;
        _firebaseOptions = firebaseOptions.Value;
        _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };
        _sourceFetcher = sourceFetcher;
    }

    /// <inheritdoc />
    public async Task Add<TData>(string key, TData data, CancellationToken cancellationToken = default)
    {
        using HttpClient client = _clientFactory.CreateClient();
        string? authToken = await Authenticate();
        HttpRequestMessage request = new()
        {
            RequestUri = new Uri($"{_firebaseOptions.Url}/{key}.json?auth={authToken}"),
            Method = HttpMethod.Post,
            Content = data.AsJsonBody()
        };

        _logger.LogInformation("Adding data of type {Data} to key {DataKey}", typeof(TData), key);
        if ((await client.SendAsync(request, cancellationToken)).IsSuccessStatusCode)
        {
            _cache.Set(key, data, _cacheOptions);
        }
        else
        {
            _logger.LogError("Failed to add data of type {Data} with key {DataKey} to database.", typeof(TData), key);
        }
    }

    /// <inheritdoc />
    public async Task Delete(string key, CancellationToken cancellationToken = default)
    {
        using HttpClient client = _clientFactory.CreateClient();
        _cache.Remove(key);
        string? authToken = await Authenticate();

        _logger.LogInformation("Deleting data from key {DataKey}", key);
        if (!(await client.DeleteAsync($"{_firebaseOptions.Url}/{key}.json?auth={authToken}", cancellationToken)).IsSuccessStatusCode)
        {
            _logger.LogError("Failed to delete data with key {DataKey} from database.", key);
        }
    }

    /// <inheritdoc />
    public async ValueTask<TData> Get<TData>(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting data of type {Data} from key {DataKey}", typeof(TData), key);

        if (_cache.TryGetValue(key, out TData? cachedData)
            && cachedData is { })
        {
            return cachedData;
        }
        else if (await GetData<TData>(key, cancellationToken) is TData dbData)
        {
            // Update cache if we fetch data from db
            _cache.Set(key, dbData, _cacheOptions);
            return dbData;
        }
        else
        {
            if (await _sourceFetcher.GetSourceData<TData>(key, cancellationToken) is { } sourceData)
            {
                if (!sourceData.IsError)
                {
                    await Add(key, sourceData.Value, cancellationToken);
                    return sourceData.Value;
                }
            }

            _logger.LogError("Source data was returned with errors: {Errors}", sourceData.Errors.ToErrorString());
            throw new InvalidDataException(typeof(TData).Name);
        }
    }

    /// <inheritdoc />
    public async Task Update<TData>(string key, TData data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating data of type {Data} at key {DataKey}", typeof(TData), key);
        await Delete(key, cancellationToken);
        await Add(key, data, cancellationToken);
    }

    /// <summary>
    /// Authenticates to Firebase in order to execute db operations.
    /// </summary>
    /// <returns>The access token of the user.</returns>
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

    /// <summary>
    /// Gets the value a specified key by first checking the cache and then the db.
    /// If no data has been previously stored the <param name="sourceFetcher"/> can be used to get data from its original source
    /// </summary>
    /// <typeparam name="TData">The type of the stored/fetched data.</typeparam>
    /// <param name="key">The key where the data is expected to be stored.</param>
    private async Task<TData?> GetData<TData>(string key, CancellationToken cancellationToken = default)
    {
        using HttpClient client = _clientFactory.CreateClient();
        string? authToken = await Authenticate();
        string url = $"{_firebaseOptions.Url}/{key}.json?auth={authToken}";

        if (await client.GetAsync(url, cancellationToken) is { IsSuccessStatusCode: true } response)
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
}

public readonly record struct FirebaseResponse(
    [property: JsonExtensionData] Dictionary<string, object>? Data);
