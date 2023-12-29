namespace TFA.Application.Services.BaseData;

public sealed record FantasyBaseData(
    [property: JsonPropertyName("players")] IReadOnlyList<Player> Players,
    [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams,
    [property: JsonPropertyName("gameweeks")] IReadOnlyList<Gameweek> Gameweeks,
    [property: JsonPropertyName("fixtures")] IReadOnlyList<Fixture> Fixtures);
