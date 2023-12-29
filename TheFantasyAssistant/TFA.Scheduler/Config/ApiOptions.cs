using System.ComponentModel.DataAnnotations;

namespace TFA.Scheduler.Config;

/// <summary>
/// Configuration for the TFA Api
/// </summary>
public class ApiOptions
{
    public const string Key = "Api";
    public const string ApiKeyHeaderValue = "x-api-key";

    [Required, Url]
    public string Url { get; set; }

    [Required]
    public string ApiKey { get; set; }
}
