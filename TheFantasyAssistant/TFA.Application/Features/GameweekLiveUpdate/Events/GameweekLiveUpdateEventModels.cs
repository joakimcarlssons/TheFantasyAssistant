using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.GameweekLiveUpdate.Events;

public sealed record GameweekLiveUpdatePresentModel(
    FantasyType FantasyType,
    int Gameweek,
    IReadOnlyList<GameweekLiveFixtureFinishedPresentModel> FinishedFixtures) : IPresentable, INotification;

public sealed record GameweekLiveFixtureFinishedPresentModel(
    int FixtureId,
    string HomeTeamName,
    string HomeTeamShortName,
    string AwayTeamName,
    string AwayTeamShortName,
    int HomeTeamScore,
    int AwayTeamScore,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> GoalScorers,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> Assisters,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> BonusPlayers,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> AttackingBonusPlayers,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> DefendingBonusPlayers);

public sealed record GameweekLiveFixtureFinishedPlayerPresentModel(
    int PlayerId,
    string DisplayName,
    string TeamShortName,
    int Value);
