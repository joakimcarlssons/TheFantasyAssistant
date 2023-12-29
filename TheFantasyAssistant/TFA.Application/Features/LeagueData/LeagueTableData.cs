namespace TFA.Application.Features.LeagueData;

public sealed record LeagueTableData(
    FantasyType FantasyType,
    IReadOnlyList<LeagueTableTeam> Teams);

public sealed record LeagueTableTeam(
    int TeamId,
    int MatchesPlayed,
    int Position,
    int Wins,
    int Draws,
    int Losses,
    int GoalsScored,
    int GoalsConceded,
    int GoalDifference,
    int Points,
    int PointsDeduction);