using System.Text.Json.Serialization;
using TFA.Infrastructure.Dtos.Gameweek;
using TFA.Infrastructure.Dtos.Player;
using TFA.Infrastructure.Dtos.Team;

namespace TFA.Infrastructure.Models;

internal sealed record FantasyBaseDataRequest(
    [property: JsonPropertyName("elements")] IReadOnlyList<FantasyPlayerRequest> Players,
    [property: JsonPropertyName("events")] IReadOnlyList<FantasyGameweekRequest> Gameweeks,
    [property: JsonPropertyName("teams")] IReadOnlyList<FantasyTeamRequest> Teams);
