using TFA.Application.Features.GameweekFinished;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.GameweekSummary;

public class GameweekSummaryTwitterBuilder : AbstractContentBuilder<GameweekSummaryData, IReadOnlyList<string>>
{
    public override Presenter Presenter => Presenter.Twitter;

    public override IReadOnlyList<IReadOnlyList<string>> Build(GameweekSummaryData data)
        => [[
            BuildGameweekSummaryStartContent(data),
            BuildGameweekSummaryTopPerformingPlayersInfoContent(data),
            .. BuildGameweekSummaryTopPerformingPlayersContent(data),
            BuildGameweekSummaryTeamsWithBestUpcomingFixturesInfoContent(data),
            .. BuildGameweekSummaryTeamsWithBestUpcomingFixturesContent(data),
            BuildGameweekSummaryEndContent()
       ]];

    private static string BuildGameweekSummaryStartContent(GameweekSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"GW{data.Gameweek.Id} #SummaryThread")
            .AppendText($"This thread provides a summary of Gameweek {data.Gameweek.Id}")
            .AppendLineBreaks(2)
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Top performing Players")
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Best upcoming fixtures")
            .AppendLineBreaks(2)
            .AppendText($"{Emoji.Thread} {Emoji.ArrowDownSmall}");

    private static string BuildGameweekSummaryEndContent()
        => new ContentBuilder()
            .AppendText("If you find this information helpful in any way please leave a like or retweet.")
            .AppendLineBreaks(2)
            .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} These threads are automatically published after each gameweek has been marked as finished.")
            .AppendText($"{Emoji.BlackSmallSquare} If you have suggestions or requests of data, please leave a comment.");

    private static string BuildGameweekSummaryTopPerformingPlayersInfoContent(GameweekSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"Top performing players GW{data.Gameweek.Id}")
            .AppendTextWithLineBreak(
                "Below are the players with the highest points accumulated during the last gameweek. " +
                "Additionally the stats of the games the player played is listed.");

    private static IEnumerable<string> BuildGameweekSummaryTopPerformingPlayersContent(GameweekSummaryData data)
        => data.TopPerformingPlayers.Select(player =>
            new ContentBuilder()
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
                    .Build());

    private static string BuildGameweekSummaryTeamsWithBestUpcomingFixturesInfoContent(GameweekSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, "Best upcoming fixtures")
            .AppendTextWithLineBreak(
                "Below are the teams with the best upcoming fixtures. " +
                "The fixture difficulty is based on the teams placing in the table and it's initial difficulty index.");

    private static IEnumerable<string> BuildGameweekSummaryTeamsWithBestUpcomingFixturesContent(GameweekSummaryData data)
        => data.TeamsWithBestUpcomingFixtures
            .Take(5)
            .Where(team => team.Opponents.Count > 0)
            .Select(team =>
                new ContentBuilder()
                    .AppendText($"{Emoji.Star} {team.Name}")
                    .AppendLineBreaks(2)
                    .AppendTextLines(opponent => 
                        $"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} {opponent.Gameweek} - {opponent.OpponentShortName} ({(opponent.IsHome ? "H" : "A")})",
                        team.Opponents)
                .Build());
}
