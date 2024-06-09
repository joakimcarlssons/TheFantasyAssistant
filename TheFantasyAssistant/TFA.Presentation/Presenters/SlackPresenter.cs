using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TFA.Application.Interfaces.Services;
using TFA.Slack;
using TFA.Slack.Config;
using TFA.Utils;

namespace TFA.Presentation.Presenters;

public sealed class SlackPresenter(
    IServiceScopeFactory scopeFactory,
    ISlackService slackService) : AbstractPresenter(scopeFactory)
{
    public override string Key => PresenterKeys.Slack;

    public override async Task Present(IPresentable presentable, CancellationToken cancellationToken = default)
    {
        foreach (SlackPresentModel model in BuildContent<SlackPresentModel>(
            presentable, 
            Presenter.Slack,
            data => data.Validate()))
        {
            await slackService.SendMessageAsync(
                model.Message,
                Env.IsDevelopment()
                    ? SlackChannels.Dev
                    : model.ChannelName);
        }
    }
}

public readonly record struct SlackPresentModel(string Message, [ConstantExpected] string ChannelName)
{
    public bool Validate() => !string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(ChannelName);
}