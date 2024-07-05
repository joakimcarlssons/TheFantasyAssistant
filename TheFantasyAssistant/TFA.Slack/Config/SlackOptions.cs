using System.ComponentModel.DataAnnotations;

namespace TFA.Slack.Config;

public sealed class SlackOptions
{
    public const string Key = "Slack";

    [Required]
    public string ApiToken { get; set; } = string.Empty;

    [Required]
    public string SigningSecret { get; set; } = string.Empty;

    public string WebhookUrl {  get; set; } = string.Empty;
}
