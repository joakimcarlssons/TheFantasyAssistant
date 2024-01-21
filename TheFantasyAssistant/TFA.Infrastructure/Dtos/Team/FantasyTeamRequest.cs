namespace TFA.Infrastructure.Dtos.Team;

internal sealed record FantasyTeamRequest(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("short_name")] string? ShortName,
    [property: JsonPropertyName("code")] int TeamCode,
    [property: JsonPropertyName("unavailable")] bool Unavailable);