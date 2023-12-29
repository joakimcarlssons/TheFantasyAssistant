using System.ComponentModel.DataAnnotations;

namespace TFA.Twitter.Config;

public class TwitterOptions
{
    public const string Key = "Twitter";

    [Required, Url]
    public string Url { get; init; } = string.Empty;

    [Required]
    public string ConsumerKey {  get; init; } = string.Empty;

    [Required]
    public string ConsumerSecret { get; init; } = string.Empty;

    [Required]
    public string AccessToken { get; init; } = string.Empty;

    [Required]
    public string TokenSecret { get; init; } = string.Empty;

    public bool SendInDev { get; init; }
}
