using System.IO;
using System.Text;
using System.Text.Json;

namespace TFA.Scraper.Contracts;

public sealed record ScraperRequest(string CallbackUrl);

public static class ScraperRequestExtensions
{
    public static ScraperRequest? ParseScraperRequest(this Stream stream)
    {
        string bodyStr = "";
        using (StreamReader sr = new(stream, Encoding.UTF8))
        {
            bodyStr = sr.ReadToEnd();
        }

        return string.IsNullOrWhiteSpace(bodyStr)
            ? default
            : JsonSerializer.Deserialize<ScraperRequest>(bodyStr);
    }
}