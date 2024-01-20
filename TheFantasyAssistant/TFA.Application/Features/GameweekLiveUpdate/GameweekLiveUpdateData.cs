namespace TFA.Application.Features.FixtureLiveUpdate;

public sealed record GameweekLiveUpdateData(
    int Gameweek,
    IReadOnlyList<GameweekLiveFinishedFixture> FinishedFixtures);

public sealed record GameweekLiveFinishedFixture(
    int FixtureId,
    GameweekLiveFinishedFixtureTeamDetails HomeTeam,
    GameweekLiveFinishedFixtureTeamDetails AwayTeam);

public sealed record GameweekLiveFinishedFixtureTeamDetails(
    int TeamId,
    string TeamName,
    string TeamShortName,
    int GoalsScored,
    IReadOnlyList<GameweekLiveFinishedFixturePlayerDetails> Players);

public sealed record GameweekLiveFinishedFixturePlayerDetails(
    int PlayerId,
    string DisplayName,
    int TotalPoints,
    int MinutesPlayed,
    int GoalsScored,
    int Assists,
    int CleanSheets,
    int AttackingBonus,
    int DefendingBonus,
    int Bonus);
