using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.WebApi;
using TFA.Slack.Config;

namespace TFA.Slack;

public interface ISlackService
{
    Task SendMessageAsync(string message, string channel);
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
}
