using TFA.Domain.Exceptions;

namespace TFA.Application.Common.Keys;

public sealed class DataKeys
{
    public const string FPL_BASE_DATA = "fpl_base_data";
    public const string FPL_BASE_DATA_TRANSFORMED = "transformed_fpl_base_data";
    public const string FPL_PLANNER_DATA = "fpl_planner_data";
    public const string FPL_LEAGUE_DATA = "fpl_league_data";
    public const string FPL_PREDICTED_PRICE_CHANGES = "fpl_predicted_price_changes";
    public const string FPL_LATEST_CHECKED_DEADLINE = "fpl_checked_deadline";
    public const string FPL_LATEST_CHECKED_FINISHED_GAMEWEEK = "fpl_checked_gameweek";
    public const string FPL_PLAYERS_NET_TRANSFERS = "fpl_players_net_transfers";
    public const string FPL_FINISHED_GAMEWEEK_FIXTURES = "fpl_finished_gw_fixtures";

    public const string FAS_BASE_DATA = "fas_base_data";
    public const string FAS_BASE_DATA_TRANSFORMED = "transformed_fas_base_data";
    public const string FAS_PLANNER_DATA = "fas_planner_data";
    public const string FAS_LEAGUE_DATA = "fas_league_data";
    public const string FAS_PREDICTED_PRICE_CHANGES = "fas_predicted_price_changes";
    public const string FAS_LATEST_CHECKED_DEADLINE = "fas_checked_deadline";
    public const string FAS_LATEST_CHECKED_FINISHED_GAMEWEEK = "fas_checked_gameweek";
    public const string FAS_PLAYER_PROJECTED_POINTS = "fas_player_projected_points";
    public const string FAS_PLAYERS_NET_TRANSFERS = "fas_players_net_transfers";
    public const string FAS_FINISHED_GAMEWEEK_FIXTURES = "fas_finished_gw_fixtures";
}

public enum KeyType
{
    BaseData,
    TransformedBaseData,
    LeagueData,
    PredictedPriceChanges,
    LastCheckedDeadline,
    LastCheckedFinishedGameweek,
    FinishedFixtures,
}

public static class DataKeysHandler
{
    public static string GetDataKey(this FantasyType fantasyType, KeyType keyType)
        => keyType switch
        {
            KeyType.BaseData =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_BASE_DATA,
                    FantasyType.Allsvenskan => DataKeys.FAS_BASE_DATA,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.TransformedBaseData =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_BASE_DATA_TRANSFORMED,
                    FantasyType.Allsvenskan => DataKeys.FAS_BASE_DATA_TRANSFORMED,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.LeagueData =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_LEAGUE_DATA,
                    FantasyType.Allsvenskan => DataKeys.FAS_LEAGUE_DATA,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.PredictedPriceChanges =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_PREDICTED_PRICE_CHANGES,
                    FantasyType.Allsvenskan => DataKeys.FAS_PREDICTED_PRICE_CHANGES,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.LastCheckedDeadline =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_LATEST_CHECKED_DEADLINE,
                    FantasyType.Allsvenskan => DataKeys.FAS_LATEST_CHECKED_DEADLINE,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.LastCheckedFinishedGameweek =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_LATEST_CHECKED_FINISHED_GAMEWEEK,
                    FantasyType.Allsvenskan => DataKeys.FAS_LATEST_CHECKED_FINISHED_GAMEWEEK,
                    _ => throw new FantasyTypeNotSupportedException()
                },
            KeyType.FinishedFixtures =>
                fantasyType switch
                {
                    FantasyType.FPL => DataKeys.FPL_FINISHED_GAMEWEEK_FIXTURES,
                    FantasyType.Allsvenskan => DataKeys.FAS_FINISHED_GAMEWEEK_FIXTURES,
                    _ => throw new FantasyTypeNotSupportedException()
                },

            _ => throw new NotImplementedException("Provided key type does not exist"),
        };

    public static FantasyType GetFantasyType(string key)
        => key switch
        {
            string when key.StartsWith("fpl") => FantasyType.FPL,
            string when key.StartsWith("fas") => FantasyType.Allsvenskan,
            _ => FantasyType.Unknown
        };
}