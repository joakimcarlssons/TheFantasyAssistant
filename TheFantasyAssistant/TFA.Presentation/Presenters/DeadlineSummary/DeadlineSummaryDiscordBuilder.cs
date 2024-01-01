using System.Text;
using TFA.Application.Features.Deadline;
using TFA.Discord.Config;
using TFA.Domain.Data;
using TFA.Domain.Exceptions;

namespace TFA.Presentation.Presenters.DeadlineSummary;

public class DeadlineSummaryDiscordBuilder : AbstractContentBuilder<DeadlineSummaryData, DiscordFilePresentModel>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<DiscordFilePresentModel> Build(DeadlineSummaryData data)
        => [BuildDeadlineSummaryContent(data)];

    private static DiscordFilePresentModel BuildDeadlineSummaryContent(DeadlineSummaryData data)
    {
        string content = new ContentBuilder()
            .AppendStandardHeader(data.FantasyType, $"GW{data.Gameweek.Id} Deadline Summary")
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- PLAYERS TO TARGET --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildPlayersToTargetContent(data))
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- PLAYERS RISKING SUSPENSION --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildPlayersRiskingSuspensionContent(data))
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- TEAMS TO TARGET --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildTeamToTargetContent(data))
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText("-- BEST UPCOMING FIXTURES  --")
            .AppendLineBreaks(2)
            .AppendText("----------------------------------------")
            .AppendLineBreaks(2)
            .AppendText(BuildTeamsWithBestUpcomingFixturesContent(data));


        return new DiscordFilePresentModel(
            $"DeadlineSummary_{data.Gameweek.Id}.txt",
            content,
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLSummaries,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanSummaries,
                _ => throw new FantasyTypeNotSupportedException()
            });
    }

    private static string BuildPlayersToTargetContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendTextLines(player =>
                $"{Emoji.Star} {player.DisplayName} #{player.TeamShortName}\n\n" +
                $"{Emoji.BlackSmallSquare} xPoints: {player.ExpectedPoints}\n" +
                $"{Emoji.BlackSmallSquare} Form: {player.Form}\n" +
                $"{Emoji.BlackSmallSquare} xGoals/90 min: {player.ExpectedGoalsPer90}\n" +
                $"{Emoji.BlackSmallSquare} xAssists/90 min: {player.ExpectedAssistsPer90}\n" +
                $"{Emoji.BlackSmallSquare} Shots/90 min: {player.ExpectedShotsPer90}\n" +
                $"{Emoji.BlackSmallSquare} Shots on target/90 min: {player.ExpectedShotsOnTargetPer90}\n" +
                $"{Emoji.BlackSmallSquare} Chances created/90 min: {player.ChancesCreatedPer90}\n" +
                $"{Emoji.BlackSmallSquare} Blocks/90 min: {player.BlocksPer90}\n" +
                $"{Emoji.BlackSmallSquare} Interceptions/90 min: {player.InterceptionsPer90}\n" +
                $"{Emoji.BlackSmallSquare} Clearances/90 min: {player.ClearancesPer90}" +
                $"\n\n",
                data.PlayersToTarget);

    private static string BuildPlayersRiskingSuspensionContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendTextLines(player =>
                $"{Emoji.YellowSquare} {player.DisplayName} #{player.TeamShortName}",
                data.PlayersRiskingSuspension);

    private static string BuildTeamToTargetContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendCustomText(() => AppendTeamsWithBestFixturesContent(data.TeamsToTarget));

    private static string BuildTeamsWithBestUpcomingFixturesContent(DeadlineSummaryData data)
        => new ContentBuilder()
            .AppendCustomText(() => AppendTeamsWithBestFixturesContent(data.TeamsWithBestUpcomingFixtures));

    private static string AppendTeamsWithBestFixturesContent(IReadOnlyList<DeadlineSummaryTeamToTarget> teams)
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
