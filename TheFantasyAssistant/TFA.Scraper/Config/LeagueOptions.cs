using System.ComponentModel.DataAnnotations;

namespace TFA.Scraper.Config;

public class LeagueOptions
{
    public const string Key = "Leagues";

    [Required]
    public string PremierLeagueUrl {  get; set; } = string.Empty;

    [Required]
    public string AllsvenskanUrl { get; set; } = string.Empty;
}
