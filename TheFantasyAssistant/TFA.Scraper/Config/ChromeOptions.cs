namespace TFA.Scraper.Config;

public class ChromeOptions
{
    public const string Key = "Chrome";
    public string ChromeExecutablePath { get; init; } = null!;
    public string UserAgent { get; init; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36";
}
