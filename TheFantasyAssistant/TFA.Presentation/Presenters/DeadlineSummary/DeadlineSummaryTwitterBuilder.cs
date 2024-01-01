using System.Text;
using TFA.Application.Features.Deadline;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.DeadlineSummary;

public class DeadlineSummaryTwitterBuilder : AbstractContentBuilder<DeadlineSummaryData, string>
{
    public override Presenter Presenter => Presenter.Twitter;

    public override IReadOnlyList<string> Build(DeadlineSummaryData data)
        => [
            BuildDeadlineStartContent(data),
            BuildPlayersToTargetInfoContent(data),
            .. BuildPlayersToTargetContent(data),
            BuildPlayersRiskingSuspensionContent(data),
            BuildTeamToTargetContent(data),
            BuildTeamsWithBestUpcomingFixturesContent(data),
            BuildDeadlineEndContent(data)
        ];

    private static string BuildDeadlineStartContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"GW{data.Gameweek.Id} #DeadlineThread")
            .AppendText($"This thread provides you with information before Gameweek {data.Gameweek.Id}")
            .AppendLineBreaks(2)
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Players to target")
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Players risking suspension")
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Teams to target")
            .AppendTextWithLineBreak($"{Emoji.Pushpin} Best upcoming fixtures")
            .AppendLineBreaks(2)
            .AppendText($"{Emoji.Thread} {Emoji.ArrowDownSmall}");

    private static string BuildDeadlineEndContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendText("If you find this information helpful in any way please leave a like or retweet.")
            .AppendLineBreaks(2)
            .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} These threads are automatically published one day before every deadline.")
            .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} If you have suggestions or requests of data, please leave a comment.")
            .AppendTextConditionally($"\n{Emoji.GlowingStar} Credit to @AllFaLytics for providing data.", 
                condition: data.FantasyType == FantasyType.Allsvenskan);

    private static string BuildPlayersToTargetInfoContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"Players To Target GW{data.Gameweek.Id}")
            .AppendText(
                "Below are the players with the highest expected points in the upcoming gameweek." +
                "To help you even further, additional statistics are added for each player.");

    private static IEnumerable<string> BuildPlayersToTargetContent(DeadlineSummaryData data)
        => data.PlayersToTarget.Select(player =>
            new ContentBuilder()
                .AppendCustomText(() => new ContentBuilder()
                    .AppendText($"{Emoji.Star} {player.DisplayName} #{player.TeamShortName}"))
                    .AppendLineBreaks(2)
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} xPoints: {player.ExpectedPoints}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Form: {player.Form}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} xGoals/90 min: {player.ExpectedGoalsPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} xAssists/90 min: {player.ExpectedAssistsPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Shots/90 min: {player.ExpectedShotsPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Shots on target/90 min: {player.ExpectedShotsOnTargetPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Chances created/90 min: {player.ChancesCreatedPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Blocks/90 min: {player.BlocksPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Interceptions/90 min: {player.InterceptionsPer90}")
                    .AppendTextWithLineBreak($"{Emoji.BlackSmallSquare} Clearances/90 min: {player.ClearancesPer90}")
                .Build());

    private static string BuildPlayersRiskingSuspensionContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, "Players Risking Suspension")
            .AppendTextLines(player =>
                $"{Emoji.YellowSquare} {player.DisplayName} #{player.TeamShortName}",
                data.PlayersRiskingSuspension);

    private static string BuildTeamToTargetContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"Best fixtures GW{data.Gameweek.Id}")
            .AppendCustomText(() => AppendTeamsWithBestFixturesContent(data.TeamsToTarget));

    private static string BuildTeamsWithBestUpcomingFixturesContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, "Best upcoming fixtures")
            .AppendCustomText(() => AppendTeamsWithBestFixturesContent(data.TeamsWithBestUpcomingFixtures));

    private static string AppendTeamsWithBestFixturesContent(IEnumerable<DeadlineSummaryTeamToTarget> teams)
    {
        StringBuilder sb = new();

        foreach (DeadlineSummaryTeamToTarget team in teams)
        {
            sb.AppendLine($"{Emoji.Star}{team.TeamName}");
            foreach (DeadlineSummaryTeamOpponent opponent in team.Opponents)
            {
                sb.AppendLine($"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} " +
                    $"{opponent.OpponentShortName} ({(opponent.IsHome ? "H" : "A")})");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
