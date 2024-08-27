using TFA.Application.Features.BaseData.Events;
using TFA.Domain.Data;
using TFA.Slack.Config;

namespace TFA.Presentation.Presenters.BaseData;

public sealed class BaseDataSlackBuilder : AbstractContentBuilder<BaseDataPresentModel, SlackPresentModel>
{
    public override Presenter Presenter => Presenter.Slack;

    public override IReadOnlyList<SlackPresentModel> Build(BaseDataPresentModel data)
        => [
            BuildPlayerPriceRiseContent(data),
            BuildPlayerPriceFallContent(data),
            BuildPlayerStatusAvailableChangeContent(data),
            BuildPlayerStatusDoubtfulChangeContent(data),
            BuildPlayerStatusUnavailableChangeContent(data),
            BuildNewPlayersContent(data),
            BuildTransferredPlayersContent(data)
        ];

    private static SlackPresentModel BuildPlayerPriceRiseContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerPriceChanges.RisingPlayers.Count, 
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.PriceRises}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.ArrowUp} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m", data.Data.PlayerPriceChanges.RisingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.PriceChanges,
                FantasyType.Allsvenskan => SlackChannels.PriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            },
            data.FantasyType);

    private static SlackPresentModel BuildPlayerPriceFallContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerPriceChanges.FallingPlayers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.PriceFallers}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.ArrowDown} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m", data.Data.PlayerPriceChanges.FallingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.PriceChanges,
                FantasyType.Allsvenskan => SlackChannels.PriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            },
            data.FantasyType);

    private static SlackPresentModel BuildPlayerStatusAvailableChangeContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerStatusChanges.AvailablePlayers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.PlayersAvailable}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.WhiteCheckMark} {player.DisplayName} #{player.TeamShortName}", data.Data.PlayerStatusChanges.AvailablePlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static SlackPresentModel BuildPlayerStatusDoubtfulChangeContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerStatusChanges.DoubtfulPlayers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.PlayersDoubtful}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.Warning} {player.DisplayName} #{player.TeamShortName} {(!string.IsNullOrWhiteSpace(player.News) ? $"- [{player.News}]" : string.Empty)}", data.Data.PlayerStatusChanges.DoubtfulPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static SlackPresentModel BuildPlayerStatusUnavailableChangeContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerStatusChanges.UnavailablePlayers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.PlayersUnavailable}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.X} {player.DisplayName} #{player.TeamShortName} {(!string.IsNullOrWhiteSpace(player.News) ? $"- [{player.News}]" : string.Empty)}", data.Data.PlayerStatusChanges.UnavailablePlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static SlackPresentModel BuildNewPlayersContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.NewPlayers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.NewPlayers}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.BustInSilhouette} {player.DisplayName} ({player.Position}) #{player.TeamShortName} - [£{player.Price}m]", data.Data.NewPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static SlackPresentModel BuildTransferredPlayersContent(BaseDataPresentModel data)
        => new(IgnoreEmptyCollections(
            data.Data.PlayerTransfers.Count,
            new ContentBuilder()
                .AppendFantasyTypeHashTag(data.FantasyType)
                .AppendText($" {BaseDataContentHeaders.TransferredPlayers}")
                .AppendLineBreaks(1)
                .AppendTextLines(player =>
                    $"{Emoji.ArrowsCounterClockwise} {player.DisplayName} [#{player.PrevTeamShortName} {Emoji.ArrowRight} #{player.NewTeamShortName}]", data.Data.PlayerTransfers)),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static string IgnoreEmptyCollections(int collectionCount, string content)
        => collectionCount == 0 ? string.Empty : content;
}
