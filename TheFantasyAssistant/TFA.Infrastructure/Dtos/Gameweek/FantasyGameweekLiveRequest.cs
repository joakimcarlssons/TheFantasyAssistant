namespace TFA.Infrastructure.Dtos.Gameweek;

public sealed record FantasyGameweekLiveRequest(
    [property: JsonPropertyName("elements")] IReadOnlyList<FantasyGameweekLivePlayerRequest>? Players);

public sealed record FantasyGameweekLivePlayerRequest(
    [property: JsonPropertyName("id")] int PlayerId,
    [property: JsonPropertyName("stats")] FantasyGameweekLivePlayerStatsRequest? GameweekStats,
    [property: JsonPropertyName("explain")] IReadOnlyList<FantasyGameweekLivePlayerDetailsRequest>? GameweekDetails);

public sealed record FantasyGameweekLivePlayerStatsRequest(
    [property: JsonPropertyName("minutes")] int MinutesPlayed,
    [property: JsonPropertyName("goals_scored")] int Goals,
    [property: JsonPropertyName("assists")] int Assists,
    [property: JsonPropertyName("clean_sheets")] int CleanSheets,
    [property: JsonPropertyName("goals_conceded")] int GoalsConceded,
    [property: JsonPropertyName("penalties_saved")] int PenaltiesSaved,
    [property: JsonPropertyName("penalties_missed")] int PenaltiesMissed,
    [property: JsonPropertyName("yellow_cards")] int YellowCards,
    [property: JsonPropertyName("red_cards")] int RedCards,
    [property: JsonPropertyName("saves")] int Saves,
    [property: JsonPropertyName("own_goals")] int OwnGoals,
    [property: JsonPropertyName("attacking_bonus")] int AttackingBonus,
    [property: JsonPropertyName("defending_bonus")] int DefendingBonus,
    [property: JsonPropertyName("winning_goals")] int WinningGoals,
    [property: JsonPropertyName("key_passes")] int KeyPasses,
    [property: JsonPropertyName("clearances_blocks_interceptions")] int ClearancesBlocksAndInterceptions,
    [property: JsonPropertyName("total_points")] int Points,
    [property: JsonPropertyName("in_dreamteam")] bool InDeamTeam,
    [property: JsonPropertyName("bps")] int Bps,
    [property: JsonPropertyName("bonus")] int Bonus);

public sealed record FantasyGameweekLivePlayerDetailsRequest(
    [property: JsonPropertyName("fixture")] int FixtureId,
    [property: JsonPropertyName("stats")] IReadOnlyList<FantasyGameweekLivePlayerDetailStatRequest> FixtureStats);

public sealed record FantasyGameweekLivePlayerDetailStatRequest(
    [property: JsonPropertyName("identifier")] string? StatType,
    [property: JsonPropertyName("value")] int Value,
    [property: JsonPropertyName("points")] int Points);
