using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Features.BaseData.Transforms;
using TFA.Domain.Data;
using TFA.Utils;

namespace TFA.Presentation.Presenters.BaseData;

public sealed class BaseDataTwitterBuilder : AbstractContentBuilder<BaseDataPresentModel, string>
{
    private FantasyType FantasyType = FantasyType.Unknown;
    public override Presenter Presenter => Presenter.Twitter;

    public override IReadOnlyList<string> Build(BaseDataPresentModel data)
    {
        FantasyType = data.FantasyType;
        return BuildTwitterContent(data);
    }

    private IReadOnlyList<string> BuildTwitterContent(BaseDataPresentModel data)
        => [
            BuildStringContent(BuildPlayerPriceChangeContent, data.Data.PlayerPriceChanges.RisingPlayers, BaseDataContentHeaders.PriceRises, Emoji.ArrowUp),
            BuildStringContent(BuildPlayerPriceChangeContent, data.Data.PlayerPriceChanges.FallingPlayers, BaseDataContentHeaders.PriceFallers, Emoji.ArrowDown),
            BuildStringContent(BuildPlayerStatusAvailableChangeContent, data.Data.PlayerStatusChanges.AvailablePlayers, BaseDataContentHeaders.PlayersAvailable, Emoji.WhiteCheckMark),
            BuildStringContent(BuildPlayerStatusNotAvailableChangeContent, data.Data.PlayerStatusChanges.DoubtfulPlayers, BaseDataContentHeaders.PlayersDoubtful, Emoji.Warning),
            BuildStringContent(BuildPlayerStatusNotAvailableChangeContent, data.Data.PlayerStatusChanges.UnavailablePlayers, BaseDataContentHeaders.PlayersUnavailable, Emoji.X),
            BuildStringContent(BuildNewPlayersContent, data.Data.NewPlayers, BaseDataContentHeaders.NewPlayers, Emoji.BustInSilhouette),
            BuildStringContent(BuildTransferredPlayersContent, data.Data.PlayerTransfers, BaseDataContentHeaders.TransferredPlayers, Emoji.ArrowsCounterClockwise),
            .. BuildDoubleGameweekContent(data.Data.DoubleGameweeks),
            BuildBlankGameweekContent(data.Data.BlankGameweeks)
        ];

    private string BuildPlayerPriceChangeContent(IReadOnlyList<PlayerPriceChange> players, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m", players);

    private string BuildPlayerStatusAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName}", players);

    private string BuildPlayerStatusNotAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName} {(!string.IsNullOrWhiteSpace(player.News) ? $"- [{player.News}]" : string.Empty)}", players);

    private string BuildNewPlayersContent(IReadOnlyList<NewPlayer> players, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} ({player.Position}) #{player.TeamShortName} - [£{player.Price}m]", players);

    private string BuildTransferredPlayersContent(IReadOnlyList<PlayerTransfer> players, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} [#{player.PrevTeamShortName} {Emoji.ArrowRight} #{player.NewTeamShortName}]", players);

    private IReadOnlyList<string> BuildDoubleGameweekContent(IReadOnlyList<DoubleGameweek> data)
        => data.Count > 0
            ? data.SplitEnumerable(4)
                .Select(x => new ContentBuilder()
                    .AppendStandardHeader(FantasyType, BaseDataContentHeaders.DoubleGameweekAnnouncement)
                    .AppendTextLines(gw =>
                        new ContentBuilder()
                            .AppendText($"{Emoji.GlowingStar} GW{gw.Gameweek} - {gw.TeamName}")
                            .AppendTextLines(opponent =>
                                $"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} {opponent.TeamShortName} {GetOpponentHomeAwayText(opponent.IsHome)}",
                                gw.Opponents)
                            .AppendLineBreaks(2),
                        x.ToList())
                    .Build())
                .ToList()
            : [string.Empty];

    private string BuildBlankGameweekContent(IReadOnlyList<BlankGameweek> data)
        => data.Count > 0
            ? new ContentBuilder()
                .AppendStandardHeader(FantasyType, BaseDataContentHeaders.BlankGameweekAnnouncement)
                .AppendTextLines(gw => $"{Emoji.WhiteCircle} GW{gw.Gameweek} - {gw.TeamName}", data)
            : string.Empty;
}
