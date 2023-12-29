using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TFA.Scraper.Models;

public class PredictedPriceChangingPlayers
{
    [JsonPropertyName("rising_players")]
    [JsonProperty("rising_players")]
    public IReadOnlyList<PredictedPlayerPriceChange>? RisingPlayers { get; set; }

    [JsonPropertyName("falling_players")]
    [JsonProperty("falling_players")]
    public IReadOnlyList<PredictedPlayerPriceChange>? FallingPlayers { get; set; }
}

public class PredictedPlayerPriceChange
{
    [JsonPropertyName("display_name")]
    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = default!;

    [JsonPropertyName("current_price")]
    [JsonProperty("current_price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("price_target")]
    [JsonProperty("price_target")]
    public decimal? PriceTarget { get; set; }

    [JsonPropertyName("team_name")]
    [JsonProperty("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("team_short_name")]
    [JsonProperty("team_short_name")]
    public string? TeamShortName { get; set; }
}
