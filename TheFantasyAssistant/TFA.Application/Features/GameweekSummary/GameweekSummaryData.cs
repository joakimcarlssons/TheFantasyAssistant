using TFA.Application.Common.Data;

namespace TFA.Application.Features.GameweekFinished;

public sealed record GameweekSummaryData(
    FantasyType FantasyType,
    Gameweek Gameweek,
    IReadOnlyList<GameweekSummaryPlayer> TopPerformingPlayers,
    IReadOnlyList<GameweekSummaryTeam> TeamsWithBestUpcomingFixtures) : SummaryData;

public sealed record GameweekSummaryPlayer(
    int PlayerId,
    string DisplayName,
    string TeamShortName,
    int Points,
    PlayerPosition Position,
    IReadOnlyList<GameweekSummaryPlayerOpponent> Opponents,
    int MinutesPlayed,
    int Goals,
    int Assists,
    int TotalShots,
    int ChancesCreated,
    decimal ExpectedGoals,
    decimal ExpectedAssists,
    int Clearances,
    int Interceptions,
    int Recoveries,
    int Saves,
    int CleanSheets,
    int AttackingBonus,
    int DefendingBonus,
    int WinningGoals,
    int KeyPasses,
    int Bonus,
    int Bps);

public sealed record GameweekSummaryPlayerOpponent(
    string TeamShortName,
    bool IsHome);

public sealed record GameweekSummaryTeam(
    int TeamId,
    string Name,
    string ShortName,
    int Position,
    int TotalDifficulty,
    IReadOnlyList<GameweekSummaryTeamOpponent> Opponents
    ) : SummaryTeamToTarget(Opponents.Count, TotalDifficulty, Position);

public sealed record GameweekSummaryTeamOpponent(
    int FixtureId,
    int Gameweek,
    string OpponentShortName,
    int FixtureDifficulty,
    bool IsHome) : SummaryTeamOpponent;


