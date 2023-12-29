using System.ComponentModel.DataAnnotations;

namespace TFA.Scraper.Config;

public class PredictedPriceChangesOptions
{
    public const string Key = "PPC";

    [Required, Url]
    public string FPLUrl { get; init; } = string.Empty;

    [Required, Url]
    public string FASUrl { get; init; } = string.Empty;
}
