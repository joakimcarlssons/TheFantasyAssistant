using DSharpPlus.Entities;
using System.Diagnostics.CodeAnalysis;
using TFA.Application.Interfaces.Services;
using TFA.Discord;
using TFA.Discord.Config;

namespace TFA.Presentation.Presenters;

public class DiscordPresenter(
    IServiceProvider serviceProvider,
    IDiscordService discordService) : AbstractPresenter(serviceProvider)
{
    public override string Key => PresenterKeys.Discord;

    public override async Task Present(IPresentable data, CancellationToken cancellationToken)
    {
        foreach (DiscordPresentModel model in BuildContent<DiscordPresentModel>(
            data, 
            Presenter.Discord, 
            x => !string.IsNullOrWhiteSpace(x.Content.Description)))
        {
            await discordService.SendMessageAsync(model.Content, model.ChannelName);
        }
    }
}

public record struct DiscordPresentModel(DiscordEmbedBuilder Content, [ConstantExpected] string ChannelName);