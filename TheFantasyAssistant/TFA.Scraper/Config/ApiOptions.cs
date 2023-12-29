using System.ComponentModel.DataAnnotations;

namespace TFA.Scraper.Config;

public class ApiOptions
{
    public const string Key = "Api";
    public const string ApiKeyHeaderValue = "x-api-key";

    [Required]
    public string ApiKey { get; init; } = string.Empty;
}
