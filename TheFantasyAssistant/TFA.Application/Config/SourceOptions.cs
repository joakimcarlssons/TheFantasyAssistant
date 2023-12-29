namespace TFA.Application.Config;

public class SourceOptions : IConfigurationOptions
{
    public string Key => "Sources";

    [Required]
    public FantasyBaseDataOptions FPL { get; set; } = null!;

    [Required]
    public FantasyBaseDataOptions FAS { get; set; } = null!;

    [Required]
    public HubOptions Hub { get; set; } = null!;

    [Required]
    public FotmobOptions Fotmob { get; set; } = null!;
}

#region Sources

public class FantasyBaseDataOptions
{
    [Required, Url]
    public string Base { get; init; } = string.Empty;

    [Required, Url]
    public string Fixtures { get; init; } = string.Empty;

    [Required, Url]
    public string PlayerHistory { get; init; } = string.Empty;

    [Required, Url]
    public string GameweekLive { get; init; } = string.Empty;

    [Required, Url]
    public string GameweekPicks { get; init; } = string.Empty;
}

public class HubOptions
{
    [Required, Url]
    public string Players { get; init; } = string.Empty;
}

public class FotmobOptions
{
    [Required, Url]
    public string BaseUrl {  get; init; } = string.Empty;

    public League PL { get; init; } = null!;
    public League Allsvenskan { get; init; } = null!;
    public Player PlayerData { get; init; } = null!;

    #region Fotmob Config Objects

    public record League(int LeagueId, int SeasonId);
    public record Player(
        string BaseUrl,
        string ExpectedGoals,
        string ExpectedGoalsPer90,
        string ExpectedAssists,
        string ExpectedAssistsPer90,
        string ShotsOnTargetPer90,
        string ShotsPer90,
        string ChancesCreated,
        string BigChancesCreated,
        string InterceptionsPer90,
        string ClearancesPer90,
        string BlocksPer90);

    #endregion
}

public class AllfalyticsOptions
{
    [Required, Url]
    public string Projections {  get; init; } = string.Empty;
}

#endregion
