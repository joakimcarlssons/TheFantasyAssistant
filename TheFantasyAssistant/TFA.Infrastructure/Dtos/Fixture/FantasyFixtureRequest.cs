using System.Text.Json.Serialization;

namespace TFA.Infrastructure.Dtos.Fixture;

public sealed record FantasyFixtureRequest(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("event")] int? GameweekId,
    [property: JsonPropertyName("team_h")] int HomeTeamId,
    [property: JsonPropertyName("team_h_difficulty")] int HomeTeamDifficulty,
    [property: JsonPropertyName("team_h_score")] int? HomeTeamScore,
    [property: JsonPropertyName("team_a")] int AwayTeamId,
    [property: JsonPropertyName("team_a_difficulty")] int AwayTeamDifficulty,
    [property: JsonPropertyName("team_a_score")] int? AwayTeamScore,
    [property: JsonPropertyName("finished")] bool IsFinished
);
