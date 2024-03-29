﻿using TFA.Infrastructure.Dtos.Fixture;

namespace TFA.Infrastructure.Dtos.Player;

public sealed record FantasyPlayerRequest(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("first_name")] string? FirstName,
    [property: JsonPropertyName("second_name")] string? LastName,
    [property: JsonPropertyName("web_name")] string? DisplayName,
    [property: JsonPropertyName("now_cost")] int Price,
    [property: JsonPropertyName("team")] int TeamId,
    [property: JsonPropertyName("form")] string? Form,
    [property: JsonPropertyName("form_rank")] int? FormRank,
    [property: JsonPropertyName("value_season")] string? SeasonValue,
    [property: JsonPropertyName("element_type")] int Position,
    [property: JsonPropertyName("chance_of_playing_this_round")] int? ChanceOfPlayingThisRound,
    [property: JsonPropertyName("chance_of_playing_next_round")] int? ChanceOfPlayingNextRound,
    [property: JsonPropertyName("selected_by_percent")] string? SelectedByPercent,
    [property: JsonPropertyName("yellow_cards")] int YellowCards,
    [property: JsonPropertyName("red_cards")] int RedCards,
    [property: JsonPropertyName("goals_scored")] int Goals,
    [property: JsonPropertyName("assists")] int Assists,
    [property: JsonPropertyName("clean_sheets")] int CleanSheets,
    [property: JsonPropertyName("clean_sheets_per_90")] decimal? CleanSheetsPerMatch,
    [property: JsonPropertyName("goals_conceded")] int GoalsConceded,
    [property: JsonPropertyName("minutes")] int MinutesPlayed,
    [property: JsonPropertyName("saves")] int Saves,
    [property: JsonPropertyName("saves_per_90")] decimal? SavesPerMatch,
    [property: JsonPropertyName("bonus")] int Bonus,
    [property: JsonPropertyName("bps")] int Bps,
    [property: JsonPropertyName("total_points")] int TotalPoints,
    [property: JsonPropertyName("points_per_game")] string? PointsPerGame,
    [property: JsonPropertyName("ep_this")] string? ExpectedPointsCurrentGameweek,
    [property: JsonPropertyName("ep_next")] string? ExpectedPointsNextGameweek,
    [property: JsonPropertyName("corners_and_indirect_freekicks_order")] int? CornersOrder,
    [property: JsonPropertyName("direct_freekicks_order")] int? FreekicksOrder,
    [property: JsonPropertyName("penalties_order")] int? PenaltiesOrder,
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("news")] string? News,
    [property: JsonPropertyName("news_added")] DateTime? NewsAdded,
    [property: JsonPropertyName("expected_goals")] string? ExpectedGoals,
    [property: JsonPropertyName("expected_goals_per_90")] decimal? ExpectedGoalsPerMatch,
    [property: JsonPropertyName("expected_assists")] string? ExpectedAssists,
    [property: JsonPropertyName("expected_assists_per_90")] decimal? ExpectedAssistsPerMatch,
    [property: JsonPropertyName("expected_goal_involvements")] string? ExpectedGoalsInvolvement,
    [property: JsonPropertyName("expected_goal_involvements_per_90")] decimal? ExpectedGoalsInvolvementPerMatch,
    [property: JsonPropertyName("expected_goals_conceded")] string? ExpectedGoalsConceded,
    [property: JsonPropertyName("expected_goals_conceded_per_90")] decimal? ExpectedGoalsConcededPerMatch,
    [property: JsonPropertyName("transfers_in")] long TransfersInTotal,
    [property: JsonPropertyName("transfers_in_event")] int TransfersInGameweek,
    [property: JsonPropertyName("transfers_out")] long TransfersOutTotal,
    [property: JsonPropertyName("transfers_out_event")] int TransfersOutGameweek,
    [property: JsonPropertyName("attacking_bonus")] int AttackingBonus,
    [property: JsonPropertyName("defending_bonus")] int DefendingBonus,
    [property: JsonPropertyName("key_passes")] int KeyPasses,
    [property: JsonPropertyName("clearances_blocks_interceptions")] int ClearancesBlocksInterceptions,
    [property: JsonPropertyName("cost_change_start_fall")] int PriceFallsTotal,
    [property: JsonPropertyName("cost_change_event_fall")] int PriceFallsGameweek,
    [property: JsonPropertyName("cost_change_start")] int PriceRisesTotal,
    [property: JsonPropertyName("cost_change_event")] int PriceRisesGameweek);

public sealed record FantasyPlayerHistoryRequest(
    [property: JsonPropertyName("fixtures")] IReadOnlyList<FantasyFixtureRequest>? Fixtures,
    [property: JsonPropertyName("history")] IReadOnlyList<FantasyPlayerFixtureHistoryRequest>? FixtureHistory);

public sealed record FantasyPlayerFixtureHistoryRequest(
    [property: JsonPropertyName("element")] int PlayerId,
    [property: JsonPropertyName("fixture")] int FixtureId,
    [property: JsonPropertyName("opponent_team")] int OpponentTeamId,
    [property: JsonPropertyName("total_points")] int Points,
    [property: JsonPropertyName("goals_scored")] int Goals,
    [property: JsonPropertyName("own_goals")] int OwnGoals,
    [property: JsonPropertyName("goals_conceded")] int GoalsConceded,
    [property: JsonPropertyName("assists")] int Assists,
    [property: JsonPropertyName("saves")] int Saves,
    [property: JsonPropertyName("clean_sheets")] int CleanSheets,
    [property: JsonPropertyName("penalties_saved")] int PenaltiesSaved,
    [property: JsonPropertyName("penalties_missed")] int PenaltiesMissed,
    [property: JsonPropertyName("bonus")] int Bonus,
    [property: JsonPropertyName("bps")] int Bps,
    [property: JsonPropertyName("attacking_bonus")] int AttackingBonus,
    [property: JsonPropertyName("defending_bonus")] int DefendingBonus,
    [property: JsonPropertyName("clearances_blocks_interceptions")] int ClearancesBlocksInterceptions,
    [property: JsonPropertyName("key_passes")] int KeyPasses,
    [property: JsonPropertyName("winning_goals")] int WinningGoals);
