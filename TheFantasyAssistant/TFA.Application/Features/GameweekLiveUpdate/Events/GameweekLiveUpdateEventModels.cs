using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.GameweekLiveUpdate.Events;

public sealed record GameweekLiveUpdatePresentModel(
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
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> HomeTeamGoalScorers,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> HomeTeamAssisters,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> AwayTeamGoalScorers,
    IReadOnlyList<GameweekLiveFixtureFinishedPlayerPresentModel> AwayTeamAssisters);

public sealed record GameweekLiveFixtureFinishedPlayerPresentModel(
    int PlayerId,
    string DisplayName,
    int Value);
