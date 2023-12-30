using Microsoft.Extensions.Logging;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Common.Notifications;

public class OutboxPublisher(
    ILogger<OutboxPublisher> logger,
    IEmailService emailService) : INotificationPublisher
{
    public Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        new Thread(async () =>
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
        }
        .Start();

        return Task.CompletedTask;
    }
}
