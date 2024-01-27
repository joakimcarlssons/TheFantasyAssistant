﻿using System.Diagnostics.CodeAnalysis;
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
            BuildStringContent(BuildPlayerPriceChangeContent, data.Data.PlayerPriceChanges.RisingPlayers, "Price Rises", Emoji.ArrowUp),
            BuildStringContent(BuildPlayerPriceChangeContent, data.Data.PlayerPriceChanges.FallingPlayers, "Price Fallers", Emoji.ArrowDown),
            BuildStringContent(BuildPlayerStatusAvailableChangeContent, data.Data.PlayerStatusChanges.AvailablePlayers, "Players Available", Emoji.WhiteCheckMark),
            BuildStringContent(BuildPlayerStatusNotAvailableChangeContent, data.Data.PlayerStatusChanges.DoubtfulPlayers, "Players Doubtful", Emoji.Warning),
            BuildStringContent(BuildPlayerStatusNotAvailableChangeContent, data.Data.PlayerStatusChanges.UnavailablePlayers, "Players Unavailable", Emoji.X),
            BuildStringContent(BuildNewPlayersContent, data.Data.NewPlayers, "New Players", Emoji.BustInSilhouette),
            BuildStringContent(BuildTransferredPlayersContent, data.Data.PlayerTransfers, "Transferred Players", Emoji.ArrowsCounterClockwise),
            .. BuildDoubleGameweekContent(data.Data.DoubleGameweeks),
            BuildBlankGameweekContent(data.Data.BlankGameweeks)
        ];

    private string BuildPlayerPriceChangeContent(IReadOnlyList<PlayerPriceChange> players, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
            .AppendStandardHeader(FantasyType, header)
            .AppendTextLines(player =>
                $"{emoji} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m", players);

    private string BuildPlayerStatusAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
            .AppendStandardHeader(FantasyType, header)
            .AppendTextLines(player =>
                $"{emoji} {player.DisplayName} #{player.TeamShortName}", players);

    private string BuildPlayerStatusNotAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
            .AppendStandardHeader(FantasyType, header)
            .AppendTextLines(player =>
                $"{emoji} {player.DisplayName} #{player.TeamShortName} {(!string.IsNullOrWhiteSpace(player.News) ? $"- [{player.News}]" : string.Empty)}", players);

    private string BuildNewPlayersContent(IReadOnlyList<NewPlayer> players, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
            .AppendStandardHeader(FantasyType, header)
            .AppendTextLines(player =>
                $"{emoji} {player.DisplayName} ({player.Position}) #{player.TeamShortName} - [£{player.Price}m]", players);

    private string BuildTransferredPlayersContent(IReadOnlyList<PlayerTransfer> players, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
            .AppendStandardHeader(FantasyType, header)
            .AppendTextLines(player =>
                $"{emoji} {player.DisplayName} [#{player.PrevTeamShortName} {Emoji.ArrowRight} #{player.NewTeamShortName}]", players);

    private IReadOnlyList<string> BuildDoubleGameweekContent(IReadOnlyList<DoubleGameweek> data)
        => data.Count > 0
            ? data.SplitEnumerable(4)
                .Select(x => new ContentBuilder()
                    .AppendStandardHeader(FantasyType, "Double Gameweek Announcements")
                    .AppendTextLines(gw =>
                        new ContentBuilder()
                            .AppendTextWithLineBreak($"{Emoji.GlowingStar} GW{gw.Gameweek} - {gw.TeamName}")
                            .AppendTextLines(opponent =>
                                $"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} {opponent.TeamShortName} ({GetOpponentHomeAwayText(opponent.IsHome)})",
                                gw.Opponents)
                            .AppendLineBreaks(2),
                        x.ToList())
                    .Build())
                .ToList()
            : [string.Empty];

    private string BuildBlankGameweekContent(IReadOnlyList<BlankGameweek> data)
        => data.Count > 0
            ? new ContentBuilder()
                .AppendStandardHeader(FantasyType, "Blank Gameweek Announcements")
                .AppendTextLines(gw => $"{Emoji.WhiteCircle} GW{gw.Gameweek} - {gw.TeamName}", data)
            : string.Empty;
}
