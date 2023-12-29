namespace TFA.Domain.Models.Teams;

public sealed record Team(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("short_name")] string ShortName,
    [property: JsonPropertyName("team_code")] int TeamCode,
    [property: JsonPropertyName("matches_played")] int? MatchesPlayed,
    [property: JsonPropertyName("table_position")] int? Position,
    [property: JsonPropertyName("wins")] int? Wins,
    [property: JsonPropertyName("draws")] int? Draws,
    [property: JsonPropertyName("losses")] int? Losses,
    [property: JsonPropertyName("goals_scored")] int? GoalsScored,
    [property: JsonPropertyName("goals_conceded")] int? GoalsConceded,
    [property: JsonPropertyName("goal_difference")] int? GoalDifference,
    [property: JsonPropertyName("points")] int? Points
) : IEntity;
