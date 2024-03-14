namespace TFA.Application.Config;

public class EmailOptions : IConfigurationOptions
{
    public string Key => "Email";

    [Required]
    public string Host { get; init; } = CommonEmailConstants.Host;

    [Required]
    public short Port { get; init; } = CommonEmailConstants.Port;

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record SmtpMessageSettings(
    string Username,
    string Password,
    string SenderEmail,
    string ReceiverEmail,
    string Subject,
    string Body,
    string Host = CommonEmailConstants.Host,
    short Port = CommonEmailConstants.Port,
    string SenderName = CommonEmailConstants.SenderName,
    string ReceiverName = "",
    bool BodyIsHTML = true,
    string PlainTextBody = "");

public sealed class CommonEmailConstants
{
    public const string Host = "smtp.gmail.com";
    public const short Port = 587;
    public const string SenderName = "The Fantasy Assistant";
}