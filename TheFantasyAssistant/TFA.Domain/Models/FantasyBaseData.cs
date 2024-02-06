using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;

namespace TFA.Domain.Models;

public sealed record FantasyBaseData(
    [property: JsonPropertyName("players")] IReadOnlyList<Player> Players,
    [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams,
    [property: JsonPropertyName("gameweeks")] IReadOnlyList<Gameweek> Gameweeks,
    [property: JsonPropertyName("fixtures")] IReadOnlyList<Fixture> Fixtures);

public record KeyedBaseData(
    IReadOnlyDictionary<int, Player> PlayersById,
    ILookup<int, Player> PlayersByTeamId,
    IReadOnlyDictionary<int, Team> TeamsById,
    IReadOnlyDictionary<string, Team> TeamsByName,
    IReadOnlyDictionary<int, Gameweek> GameweeksById,
    IReadOnlyDictionary<int, Fixture> FixturesById,
    ILookup<int, Fixture> FixturesByGameweekId,
    ILookup<int, Fixture> FixturesByHomeTeamId,
    ILookup<int, Fixture> FixturesByAwayTeamId);
