using TFA.Application.Features.BaseData.Events;
using TFA.Domain.Data;
using TFA.Slack.Config;

namespace TFA.Presentation.Presenters.BaseData;

public sealed class BaseDataSlackBuilder : AbstractBaseDataContentBuilder<SlackPresentModel>
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

    private SlackPresentModel BuildPlayerPriceRiseContent(BaseDataPresentModel data)
        => new(BuildPlayerPriceChangeContent(
            data.Data.PlayerPriceChanges.RisingPlayers, data.FantasyType, BaseDataContentHeaders.PriceRises, Emoji.ArrowUp),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.PriceChanges,
                FantasyType.Allsvenskan => SlackChannels.PriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildPlayerPriceFallContent(BaseDataPresentModel data)
        => new(BuildPlayerPriceChangeContent(
            data.Data.PlayerPriceChanges.FallingPlayers, data.FantasyType, BaseDataContentHeaders.PriceFallers, Emoji.ArrowDown),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.PriceChanges,
                FantasyType.Allsvenskan => SlackChannels.PriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildPlayerStatusAvailableChangeContent(BaseDataPresentModel data)
        => new(BuildPlayerStatusAvailableChangeContent(
            data.Data.PlayerStatusChanges.AvailablePlayers, data.FantasyType, BaseDataContentHeaders.PlayersAvailable, Emoji.WhiteCheckMark),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildPlayerStatusDoubtfulChangeContent(BaseDataPresentModel data)
        => new(BuildPlayerStatusNotAvailableChangeContent(
            data.Data.PlayerStatusChanges.DoubtfulPlayers, data.FantasyType, BaseDataContentHeaders.PlayersDoubtful, Emoji.Warning),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildPlayerStatusUnavailableChangeContent(BaseDataPresentModel data)
        => new(BuildPlayerStatusNotAvailableChangeContent(
            data.Data.PlayerStatusChanges.UnavailablePlayers, data.FantasyType, BaseDataContentHeaders.PlayersUnavailable, Emoji.X),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildNewPlayersContent(BaseDataPresentModel data)
        => new(BuildNewPlayersContent(
            data.Data.NewPlayers, data.FantasyType, BaseDataContentHeaders.NewPlayers, Emoji.BustInSilhouette),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private SlackPresentModel BuildTransferredPlayersContent(BaseDataPresentModel data)
        => new(BuildTransferredPlayersContent(
            data.Data.PlayerTransfers, data.FantasyType, BaseDataContentHeaders.TransferredPlayers, Emoji.ArrowsCounterClockwise),
            data.FantasyType switch
            {
                FantasyType.FPL => SlackChannels.Updates,
                FantasyType.Allsvenskan => SlackChannels.Updates,
                _ => throw new FantasyTypeNotSupportedException()
            });
}
