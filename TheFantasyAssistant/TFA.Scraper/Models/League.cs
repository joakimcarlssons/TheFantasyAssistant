using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TFA.Scraper.Models;

public class League
{
    [JsonPropertyName("teams")]
    [JsonProperty("teams")]
    public HashSet<LeagueTeam> Teams { get; set; } = new();
}

public class LeagueTeam
{
    [JsonPropertyName("position")]
    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonPropertyName("matches_played")]
    [JsonProperty("matches_played")]
    public int MatchesPlayed { get; set; }

    [JsonPropertyName("wins")]
    [JsonProperty("wins")]
    public int Wins { get; set; }

    [JsonPropertyName("draws")]
    [JsonProperty("draws")]
    public int Draws { get; set; }

    [JsonPropertyName("losses")]
    [JsonProperty("losses")]
    public int Losses { get; set; }

    [JsonPropertyName("goals_scored")]
    [JsonProperty("goals_scored")]
    public int GoalsScored { get; set; }

    [JsonPropertyName("goals_conceded")]
    [JsonProperty("goals_conceded")]
    public int GoalsConceded { get; set; }

    [JsonPropertyName("goal_difference")]
    [JsonProperty("goal_difference")]
    public int GoalDifference { get; set; }

    [JsonPropertyName("points")]
    [JsonProperty("points")]
    public int Points { get; set; }
}
