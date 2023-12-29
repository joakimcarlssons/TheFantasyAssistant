using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TFA.Application.Config;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Common.Notifications;

public class OutboxPublisher(
    ILogger<OutboxPublisher> logger,
    IEmailService emailService) : INotificationPublisher
{
    public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        Thread thread = new(async () =>
        {
            foreach (NotificationHandlerExecutor handler in handlerExecutors)
            {
                try
                {
                    await handler.HandlerCallback(notification, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError("{Exception}", ex.Message);
                    await emailService.SendExceptionEmail(ex);
                    throw;
                }
            }
        })
        {
            IsBackground = true,
        };

        thread.Start();
        return Task.CompletedTask;
    }
}
