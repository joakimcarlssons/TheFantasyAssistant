﻿using TFA.Application.Common.Data;

namespace TFA.Application.Features.Deadline;

public sealed record DeadlineSummaryData(
    FantasyType FantasyType,
    Gameweek Gameweek,
    IReadOnlyList<DeadlineSummaryPlayerToTarget> PlayersToTarget,
    IReadOnlyList<DeadlineSummaryPlayerRiskingSuspension> PlayersRiskingSuspension,
    IReadOnlyList<DeadlineSummaryTeamToTarget> TeamsToTarget,
    IReadOnlyList<DeadlineSummaryTeamToTarget> TeamsWithBestUpcomingFixtures) : SummaryData;


public sealed record DeadlineSummaryPlayerToTarget(
    int PlayerId,
    int TeamId,
    string TeamShortName,
    string DisplayName,
    decimal Price,
    string SelectedByPercent,
    string ExpectedPoints,
    string Form,
    double ExpectedGoals,
    double ExpectedGoalsPer90,
    double ExpectedAssists,
    double ExpectedAssistsPer90,
    double ShotsOnTargetPer90,
    double ShotsPer90,
    double GoalsPerShotPercent,
    double ChancesCreated,
    double ChancesCreatedPer90,
    double BigChancesCreated,
    double InterceptionsPer90,
    double ClearancesPer90,
    double BlocksPer90);

public sealed record DeadlineSummaryPlayerRiskingSuspension(
    int PlayerId,
    string DisplayName,
    int TeamId,
    string TeamName,
    string TeamShortName);


public sealed record DeadlineSummaryTeamToTarget(
    int TeamId,
    string TeamName,
    string TeamShortName,
    int Position,
    int TotalDifficulty,
    IReadOnlyList<DeadlineSummaryTeamOpponent> Opponents
    ) : SummaryTeamToTarget(Opponents.Count, TotalDifficulty, Position);

public sealed record DeadlineSummaryTeamOpponent(
    int FixtureId,
    int Gameweek,
    string OpponentShortName,
    bool IsHome,
    bool IsDouble,
    bool IsBlank,
    int FixtureDifficulty) : SummaryTeamOpponent;
