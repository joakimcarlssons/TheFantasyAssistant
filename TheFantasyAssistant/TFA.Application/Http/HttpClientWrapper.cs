using System.Text.Json;

namespace TFA.Application.Http;

public static class HttpClientWrapper
{
    public static async Task<ErrorOr<T>> TryGetAsJsonAsync<T>(this HttpClient client, string url, CancellationToken cancellationToken)
    {
        if (await client.GetAsync(url, cancellationToken) is { IsSuccessStatusCode: true } response)
        {
            if (JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(cancellationToken)) is { } data)
            {
                return data;
            }
            else
            {
                return Error.Validation();
            }
        }
        else
        {
            return Error.Failure();
        }
    }
}
