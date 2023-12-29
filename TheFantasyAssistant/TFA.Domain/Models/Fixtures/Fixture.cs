namespace TFA.Domain.Models.Fixtures;

public sealed record Fixture(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("gameweek")] int? GameweekId,
    [property: JsonPropertyName("home_team_id")] int HomeTeamId,
    [property: JsonPropertyName("home_team_difficulty")] int HomeTeamDifficulty,
    [property: JsonPropertyName("home_team_score")] int? HomeTeamScore,
    [property: JsonPropertyName("away_team_id")] int AwayTeamId,
    [property: JsonPropertyName("away_team_difficulty")] int AwayTeamDifficulty,
    [property: JsonPropertyName("away_team_score")] int? AwayTeamScore,
    [property: JsonPropertyName("is_finished")] bool IsFinished
) : IEntity;

