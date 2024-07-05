using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.WebApi;
using TFA.Slack.Config;

namespace TFA.Slack;

public interface ISlackService
{
    Task SendMessageAsync(string message, string channel);
    Task SendWebhookMessageAsync(string message);
}

public class SlackService(IOptions<SlackOptions> options) : ISlackService
{
    private readonly ISlackApiClient Client = new SlackServiceBuilder()
            .UseApiToken(options.Value.ApiToken)
            .GetApiClient();

    public async Task SendMessageAsync(string message, string channel)
    {
        await Client.Chat.PostMessage(new Message
        {
            Text = message,
            Channel = channel
        });
    }

    public async Task SendWebhookMessageAsync(string message)
    {
        if (!string.IsNullOrWhiteSpace(options.Value.WebhookUrl))
        {
            await Client.PostToWebhook(options.Value.WebhookUrl, new Message
            {
                Text = message
            });
        }
    }
}
