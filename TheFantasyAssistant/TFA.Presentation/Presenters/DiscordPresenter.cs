﻿using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
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