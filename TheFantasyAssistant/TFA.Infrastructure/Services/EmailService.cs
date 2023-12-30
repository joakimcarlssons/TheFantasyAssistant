using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using System.Net.Mail;
using TFA.Application.Config;

namespace TFA.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailOptions _options;

    public static IEmailService? Instance;

    public EmailService(ILogger<EmailService> logger, IOptions<EmailOptions> options)
    {
        Instance = this;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SendResponse?> Send(SmtpMessageSettings emailSettings)
    {
        try
        {
            if (!Env.IsProduction())
            {
                // Only send emails in production;
                _logger.LogInformation("Sending email {Email}", emailSettings.Body);
                return null;
            }

            Email.DefaultSender = SetDefaultSender();
            return await Email
                .From(emailSettings.SenderEmail, emailSettings.SenderName)
                .To(emailSettings.ReceiverEmail)
                .Subject(emailSettings.Subject)
                .Body(emailSettings.Body, emailSettings.BodyIsHTML)
                .PlaintextAlternativeBody(emailSettings.PlainTextBody)
                .SendAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Exception}", ex.Message);
            throw;
        }
    }

    public Task<SendResponse?> Send(EmailMessage email)
    {
        // Construct a custom subject if there is a EmailType specified
        string subject = email.EmailType == EmailType.None
            ? email.Subject
            : $"{email.EmailType.GetConstantValue<string, EmailTypes>()}: {email.Subject}";

        return Send(new SmtpMessageSettings(
            _options.Username,
            _options.Password,
            _options.Username,
            email.Receiver,
            subject,
            email.Message));
    }

    public Task<SendResponse?> SendExceptionEmail(Exception ex)
        => Send(new EmailMessage(
            EmailType.Error,
            _options.Username,
            ex.ConstructExceptionSubject(),
            ex.ConstructExceptionMessage()));

    public Task<SendResponse?> SendErrorMessage(string subject, string body)
        => Send(new EmailMessage(
            EmailType.Error,
            _options.Username,
            subject,
            body));

    private SmtpSender SetDefaultSender()
        => new(() => new SmtpClient(_options.Host)
        {
            Port = _options.Port,
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = true
        });
}

public static class EmailExtensions
{
    public static string ConstructExceptionSubject(this Exception ex)
        => $"An exception was thrown: {ex.GetType().Name}";

    public static string ConstructExceptionMessage(this Exception ex)
        => $"{ex.Message}\n\n{ex.StackTrace}";
}
