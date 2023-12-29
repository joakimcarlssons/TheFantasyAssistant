// Don't remove, only commented out because it's only used in PROD
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TFA.Scraper.Config;

namespace TFA.Scraper.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an email to ourselves. Primarily used for error logging.
    /// </summary>
    /// <param name="content">The content of the email.</param>
    Task SendAsync(string subject, string content);
}

internal class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string subject, string content)
    {

        // Don't send emails in dev
#if !DEBUG

        Email.DefaultSender = SetDefaultSender();
        await Email
            .From(_options.Username, CommonEmailConstants.SenderName)
            .To(_options.Username)
            .Subject(subject)
            .Body(content)
            .PlaintextAlternativeBody(content)
            .SendAsync();
#endif

        Console.WriteLine(content);
        await Task.CompletedTask;
    }

    private SmtpSender SetDefaultSender()
        => new(() => new SmtpClient(_options.Host)
        {
            Port = _options.Port,
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = true
        });
}

public sealed class EmailTypes
{
    public const string Warning = "WARNING";
    public const string Error = "ERROR";
}
