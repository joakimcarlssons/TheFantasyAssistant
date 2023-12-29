using DSharpPlus.Entities;
using TFA.Application.Features.BaseData.Events;
using TFA.Discord.Config;
using TFA.Domain.Data;
using TFA.Domain.Exceptions;

namespace TFA.Presentation.Presenters.BaseData;

public sealed class BaseDataDiscordBuilder : AbstractContentBuilder<BaseDataPresentModel, DiscordPresentModel>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<DiscordPresentModel> Build(BaseDataPresentModel data)
        => [
            BuildPlayerPriceRiseContent(data),
            BuildPlayerPriceFallContent(data),
            BuildPlayerStatusAvailableChangeContent(data),
            BuildPlayerStatusDoubtfulChangeContent(data),
            BuildPlayerStatusUnavailableChangeContent(data),
            BuildNewPlayersContent(data),
            BuildTransferredPlayersContent(data)
        ];

    private static DiscordPresentModel BuildPlayerPriceRiseContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, $"Price Rises"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.ArrowUp} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m",
                    data.Data.PlayerPriceChanges.RisingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLPriceChanges,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanPriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildPlayerPriceFallContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, $"Price Fallers"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.ArrowDown} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m",
                    data.Data.PlayerPriceChanges.FallingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLPriceChanges,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanPriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildPlayerStatusAvailableChangeContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, "Players Available"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                 .AppendTextLines(player =>
                    $"{Emoji.WhiteCheckMark} {player.DisplayName} #{player.TeamShortName}",
                    data.Data.PlayerStatusChanges.AvailablePlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLUpdates,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanUpdates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildPlayerStatusDoubtfulChangeContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, "Players Doubtful"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.Warning} {player.DisplayName} #{player.TeamShortName}",
                    data.Data.PlayerStatusChanges.DoubtfulPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLUpdates,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanUpdates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildPlayerStatusUnavailableChangeContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, "Players Unavailable"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.X} {player.DisplayName} #{player.TeamShortName}",
                    data.Data.PlayerStatusChanges.UnavailablePlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLUpdates,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanUpdates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildNewPlayersContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, "New Players"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.BustInSilhouette} {player.DisplayName} ({player.Position}) #{player.TeamShortName} - [£{player.Price}m]",
                    data.Data.NewPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLUpdates,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanUpdates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildTransferredPlayersContent(BaseDataPresentModel data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, "Transferred Players"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.ArrowsCounterClockwise} {player.DisplayName} [#{player.PrevTeamShortName} {Emoji.ArrowRight} #{player.NewTeamShortName}]",
                    data.Data.PlayerTransfers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLUpdates,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanUpdates,
                _ => throw new FantasyTypeNotSupportedException()
            });
}
