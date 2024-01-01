using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TFA.Application.Interfaces.Services;
using TFA.Discord;

namespace TFA.Presentation.Presenters;

public class DiscordPresenter(
    IServiceScopeFactory scopeFactory,
    IDiscordService discordService) : AbstractPresenter(scopeFactory)
{
    public override string Key => PresenterKeys.Discord;

    public override async Task Present(IPresentable data, CancellationToken cancellationToken)
    {
        foreach (DiscordEmbedPresentModel model in BuildContent<DiscordEmbedPresentModel>(
            data, 
            Presenter.Discord, 
            x => !string.IsNullOrWhiteSpace(x.Content.Description)))
        {
            await discordService.SendMessageAsync(model.Content, model.ChannelName);
        }

        foreach (DiscordFilePresentModel content in BuildContent<DiscordFilePresentModel>(data, Presenter.Discord))
        {
            using MemoryStream fileStream = new(Encoding.UTF8.GetBytes(content.Content));
            await discordService.SendMessageAsync(new DiscordMessageBuilder()
                    .AddFiles(new Dictionary<string, Stream> { { content.FileName, fileStream } }),
                content.ChannelName);
        }
    }
}

public record struct DiscordEmbedPresentModel(DiscordEmbedBuilder Content, [ConstantExpected] string ChannelName);

/// <summary>
/// Files is to be used in order to get collapsable content in Discord.
/// The content will also be available for download in Discord.
/// </summary>
public record struct DiscordFilePresentModel(string FileName, string Content, [ConstantExpected] string ChannelName);