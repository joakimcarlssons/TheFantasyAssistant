using FluentEmail.Core.Models;
using TFA.Application.Config;

namespace TFA.Application.Interfaces.Services;

public interface IEmailService
{
    Task<SendResponse?> Send(SmtpMessageSettings settings);
    Task<SendResponse?> Send(EmailMessage email);
    Task<SendResponse?> SendExceptionEmail(Exception ex);
    Task<SendResponse?> SendErrorMessage(string subject, string body);
}

public sealed record EmailMessage(
    EmailType EmailType,
    string Receiver,
    string Subject,
    string Message,
    bool IncludeTime = true,
    bool IsHTML = true);

public sealed class EmailTypes
{
    public const string Info = "INFO";
    public const string Error = "ERROR";
    public const string Warning = "WARNING";
}

public enum EmailType
{
    None,
    Info,
    Warning,
    Error
}
