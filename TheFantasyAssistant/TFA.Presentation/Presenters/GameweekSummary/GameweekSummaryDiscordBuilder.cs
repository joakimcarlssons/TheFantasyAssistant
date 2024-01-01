using TFA.Application.Features.GameweekFinished;
using TFA.Discord.Config;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.GameweekSummary;

public class GameweekSummaryDiscordBuilder : AbstractContentBuilder<GameweekSummaryData, DiscordFilePresentModel>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<DiscordFilePresentModel> Build(GameweekSummaryData data)
        => [BuildGameweekSummaryContent(data)];

    private static DiscordFilePresentModel BuildGameweekSummaryContent(GameweekSummaryData data)
    {
        string content = new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"GW{data.Gameweek.Id} Summary")
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- TOP PERFORMING PLAYERS --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildToPerformingPlayersContent(data))
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- BEST UPCOMING FIXTURES --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildTeamsWithBestUpcomingFixturesContent(data));

        return new DiscordFilePresentModel(
            $"GameweekSummary_{data.Gameweek.Id}.txt",
            content,
            DiscordChannels.Dev);
    }

    private static string BuildToPerformingPlayersContent(GameweekSummaryData data)
        => new ContentBuilder()
            .AppendTextLines(player => new ContentBuilder()
                .AppendCustomText(() => new ContentBuilder()
                    .AppendTextWithLineBreak($"{Emoji.GlowingStar} {player.DisplayName} #{player.TeamShortName}")
                    .AppendText(Emoji.Vs)
                    .AppendCustomText(() => string.Join(",", player.Opponents
                        .Select(opponent => $" {opponent.TeamShortName} ({(opponent.IsHome ? "H" : "A")})")))
                    .AppendLineBreaks(2)
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Total Points: {player.Points}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Minutes Played: {player.MinutesPlayed}")
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Goals: {player.Goals}\n", player.Goals > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Winning Goals: {player.WinningGoals}\n", player.WinningGoals > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Assists: {player.Assists}\n", player.Assists > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Saves: {player.Saves}\n", player.Saves > 0 && player.Position == PlayerPosition.Goalkeeper)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Clean Sheets: {player.CleanSheets}\n", player.CleanSheets > 0 && player.Position != PlayerPosition.Attacker)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Bonus: {player.Bonus}\n", player.Bonus > 0 && data.FantasyType == FantasyType.FPL)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Off. Bonus: {player.AttackingBonus}\n", player.AttackingBonus > 0 && data.FantasyType == FantasyType.Allsvenskan)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Def. Bonus: {player.DefendingBonus}\n", player.DefendingBonus > 0 && data.FantasyType == FantasyType.Allsvenskan)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} xG: {player.ExpectedGoals}\n", player.ExpectedGoals > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} xA: {player.ExpectedAssists}\n", player.ExpectedAssists > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Shots: {player.TotalShots}\n", player.TotalShots > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Key Passes: {player.KeyPasses}\n", player.KeyPasses > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Recoveries: {player.Recoveries}\n", player.Recoveries > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Interceptions: {player.Interceptions}\n", player.Interceptions > 0)
                    .AppendTextConditionally($"{Emoji.BlackSmallSquare} Clearances: {player.Clearances}", player.Clearances > 0))
                .Build(),
                data.TopPerformingPlayers);

    private static string BuildTeamsWithBestUpcomingFixturesContent(GameweekSummaryData data)
        => new ContentBuilder()
            .AppendTextLines(team => new ContentBuilder()
                    .AppendText($"{Emoji.Star} {team.Name}")
                    .AppendTextLines(opponent =>
                        $"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} {opponent.Gameweek} - {opponent.OpponentShortName} ({(opponent.IsHome ? "H" : "A")})",
                        team.Opponents)
                    .AppendLineBreaks(2)
                .Build(),
                data.TeamsWithBestUpcomingFixtures
                    .Where(x => x.Opponents.Count > 0)
                    .ToList());
}
