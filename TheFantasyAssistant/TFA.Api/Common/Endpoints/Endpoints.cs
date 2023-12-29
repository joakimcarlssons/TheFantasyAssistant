namespace TFA.Api.Common.Endpoints;

public sealed class Endpoints
{
    private const string Prefix = "api/";
    private const string FPL = "fpl";
    private const string Allsvenskan = "fas";
    private const string BaseData = "bd/";
    private const string PredictedPriceChange = "ppc";
    private const string Refresh = "refresh";

    // FPL Endpoints
    public const string FPLBaseDataRefresh = $"{Prefix}{BaseData}{FPL}/{Refresh}";
    public const string FPLPlayers = $"{Prefix}{BaseData}{FPL}/players";
    public const string FPLTeams = $"{Prefix}{BaseData}{FPL}/teams";
    public const string FPLGameweeks = $"{Prefix}{BaseData}{FPL}/gameweeks";
    public const string FPLFixtures = $"{Prefix}{BaseData}{FPL}/fixtures";
    public const string FPLLeagueTableRefresh = $"{Prefix}{FPL}/league/table/{Refresh}";

    public const string FPLPredictedPriceChanges = $"{Prefix}{FPL}/{PredictedPriceChange}";
    public const string FPLDeadline = $"{Prefix}{FPL}/deadline";
    public const string FPLGameweekFinished = $"{Prefix}{FPL}/gw-finished";

    // Allsvenskan endpoints
    public const string FASBaseDataRefresh = $"{Prefix}{BaseData}{Allsvenskan}/{Refresh}";
    public const string FASPlayers = $"{Prefix}{BaseData}{Allsvenskan}/players";
    public const string FASTeams = $"{Prefix}{BaseData}{Allsvenskan}/teams";
    public const string FASGameweeks = $"{Prefix}{BaseData}{Allsvenskan}/gameweeks";
    public const string FASFixtures = $"{Prefix}{BaseData}{Allsvenskan}/fixtures";
    public const string FASDeadline = $"{Prefix}{Allsvenskan}/deadline";
    public const string FASGameweekFinished = $"{Prefix}{Allsvenskan}/gw-finished";
    public const string FASLeagueTableRefresh = $"{Prefix}{Allsvenskan}/league/table/{Refresh}";

    // System endpoints
    public const string StayAlive = $"{Prefix}stay-alive";
}

public sealed class EndpointTags
{
    public const string Development = "Development";

    public const string BaseDataRefresh = "Base Data Refresh";
    public const string FPLBaseData = "FPL Base Data";
    public const string FASBaseData = "Allsvenskan Base Data";
    public const string LeagueData = "League Data";
    public const string PredictedPriceChanges = "Predicted Price Changes";
    public const string Summaries = "Summaries";
    public const string System = "System";
}