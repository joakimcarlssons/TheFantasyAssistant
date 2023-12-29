using System.Text.Json.Serialization;

namespace TFA.Infrastructure.Dtos.Player;

public sealed record HubPlayerRequest(
    [property: JsonPropertyName("team")] HubPlayerTeam Team,
    [property: JsonPropertyName("data")] HubPlayerData Details,
    [property: JsonPropertyName("fpl")] FantasyPlayerRequest PlayerData,
    [property: JsonPropertyName("elite_ownership")] int EffectiveOwnership);

public sealed record HubPlayerTeam(
    [property: JsonPropertyName("code")] int TeamCode,
    [property: JsonPropertyName("name")] string TeamName,
    [property: JsonPropertyName("codeName")] string TeamShortName);

public sealed record HubPlayerData(
    [property: JsonPropertyName("priceInfo")] HubPlayerPriceInfo? PriceInfo,
    [property: JsonPropertyName("predictions")] IReadOnlyList<HubPlayerGameweekPrediction> Predictions,
    [property: JsonPropertyName("next_gw_xmins")] int ExpectedMinutesNextGameweek,
    [property: JsonPropertyName("prediction4GW")] decimal PredictedPointsNextFourGameweeks);

public sealed record HubPlayerPriceInfo(
    [property: JsonPropertyName("Target")] decimal PriceTarget);

public sealed record HubPlayerGameweekPrediction(
    [property: JsonPropertyName("gw")] int GameweekId,
    [property: JsonPropertyName("xmins")] int ExpectedMinutes,
    [property: JsonPropertyName("predicted_pts")] decimal PredictedPoints);
