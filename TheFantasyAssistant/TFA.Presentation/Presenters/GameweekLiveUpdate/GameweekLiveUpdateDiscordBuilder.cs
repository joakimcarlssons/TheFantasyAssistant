using DSharpPlus.Entities;
using TFA.Application.Features.GameweekLiveUpdate.Events;
using TFA.Discord.Config;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.GameweekLiveUpdate;

public sealed class GameweekLiveUpdateDiscordBuilder : AbstractContentBuilder<GameweekLiveUpdatePresentModel, DiscordEmbedPresentModel>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<DiscordEmbedPresentModel> Build(GameweekLiveUpdatePresentModel data)
        => [.. BuildFinishedFixturesContent(data)];

    private static IReadOnlyList<DiscordEmbedPresentModel> BuildFinishedFixturesContent(GameweekLiveUpdatePresentModel data)
        => data.FinishedFixtures.Select(fixture =>
            new DiscordEmbedPresentModel(new DiscordEmbedBuilder()
                .WithTitle($"FT: {fixture.HomeTeamName} {fixture.HomeTeamScore} - {fixture.AwayTeamScore} {fixture.AwayTeamName}")
                .WithFooter($"{NowDate} - GW{data.Gameweek}")
                .WithDescription(new ContentBuilder()

                    .AppendTextConditionally($"GOALS\n{new string('-', 15)}", fixture.GoalScorers.Count > 0)
                    .AppendTextConditionally(
                        new ContentBuilder().AppendTextLines(player => $"{player.Value}  -  {player.DisplayName} ({player.TeamShortName})", fixture.GoalScorers),
                        fixture.GoalScorers.Count > 0)
                    .AppendLineBreaks(2)

                    .AppendTextConditionally($"ASSISTS\n{new string('-', 15)}", fixture.Assisters.Count > 0)
                    .AppendTextConditionally(
                        new ContentBuilder().AppendTextLines(player => $"{player.Value}  -  {player.DisplayName} ({player.TeamShortName})", fixture.Assisters),
                        fixture.Assisters.Count > 0)
                    .AppendLineBreaks(2)

                    .AppendTextConditionally($"BONUS\n{new string('-', 15)}", fixture.BonusPlayers.Count > 0)
                    .AppendTextConditionally(
                        new ContentBuilder().AppendTextLines(player => $"{player.Value}  -  {player.DisplayName} ({player.TeamShortName})", fixture.BonusPlayers), 
                        fixture.BonusPlayers.Count > 0)
                    .AppendLineBreaks(2)

                    .AppendTextConditionally($"ATT. BONUS\n{new string('-', 15)}", fixture.AttackingBonusPlayers.Count > 0)
                    .AppendTextConditionally(
                        new ContentBuilder().AppendTextLines(player => $"{player.Value}  -  {player.DisplayName} ({player.TeamShortName})", fixture.AttackingBonusPlayers),
                        fixture.AttackingBonusPlayers.Count > 0)
                    .AppendLineBreaks(2)

                    .AppendTextConditionally($"DEF. BONUS\n{new string('-', 15)}", fixture.DefendingBonusPlayers.Count > 0)
                    .AppendTextConditionally(
                        new ContentBuilder().AppendTextLines(player => $"{player.Value}  -  {player.DisplayName} ({player.TeamShortName})", fixture.DefendingBonusPlayers),
                        fixture.DefendingBonusPlayers.Count > 0)
                    .AppendLineBreaks(2)

                ), data.FantasyType switch 
                {
                    FantasyType.FPL => DiscordChannels.FPLFixtureResults,
                    FantasyType.Allsvenskan => DiscordChannels.AllsvenskanFixtureResults,
                    _ => throw new FantasyTypeNotSupportedException()
                })).ToList();
}
