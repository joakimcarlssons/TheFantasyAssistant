using System.ComponentModel.DataAnnotations;
using TFA.Application.Config;

namespace TFA.Client.Config;

public class FirebaseOptions : IConfigurationOptions
{
    public string Key => "Firebase";

    [Required, Url]
    public string Url { get; set; } = string.Empty;

    [Required]
    public string AuthDomain { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
