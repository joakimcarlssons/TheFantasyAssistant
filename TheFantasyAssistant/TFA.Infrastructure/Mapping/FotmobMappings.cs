using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TFA.Application.Common.Data;
using TFA.Domain.Exceptions;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fotmob;

namespace TFA.Infrastructure.Mapping;

internal record struct FixtureTeamDetailsMappingSource(
        Team Team, bool IsHome, FotmobFixtureDetails Fixture,
        Dictionary<string, FotmobFixtureDetailsSubStat>? TopStatsByKey,
        Dictionary<string, FotmobFixtureDetailsSubStat>? ShotStatsByKey,
        FotmobLineUp? LineUp,
        IReadOnlyList<Player> TeamPlayers);
internal record struct FixtureTeamDetailsLineUpMappingSource(FotmobLineUp? LineUp, int FotmobTeamId, Team Team, IReadOnlyList<Player> TeamPlayers);
internal record struct FixtureTeamPlayerDetailsMappingSource(FotmobLineUpPlayer FotmobPlayer, Player? FantasyPlayer);

public class FotmobMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // => FixtureTeamDetails
        config.ForType<FixtureTeamDetailsMappingSource, FixtureTeamDetails>()
            .MapToConstructor(true)
            .Map(dest => dest.TeamId, src => src.Team.Id)
            .Map(dest => dest.FotmobTeamId, src => src.IsHome ? src.Fixture.HomeTeam.FotmobTeamId : src.Fixture.AwayTeam.FotmobTeamId)
            .Map(dest => dest.TeamName, src => src.Team.Name)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName)
            .Map(dest => dest.Possession, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.POSSESSION, src.IsHome))
            .Map(dest => dest.ExpectedGoals, src => GetFotmobFixtureSubSubStatValue<string>(src.TopStatsByKey, FotmobKeys.SubStats.EXPECTED_GOALS, src.IsHome).ToDecimal(decimal.Zero))
            .Map(dest => dest.TotalShots, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.SHOTS, src.IsHome))
            .Map(dest => dest.ShotsOffTarget, src => GetFotmobFixtureSubSubStatValue<int>(src.ShotStatsByKey, FotmobKeys.SubStats.SHOTS_OFF_TARGET, src.IsHome))
            .Map(dest => dest.ShotsOnTarget, src => GetFotmobFixtureSubSubStatValue<int>(src.ShotStatsByKey, FotmobKeys.SubStats.SHOTS_ON_TARGET, src.IsHome))
            .Map(dest => dest.ShotsInsideBox, src => GetFotmobFixtureSubSubStatValue<int>(src.ShotStatsByKey, FotmobKeys.SubStats.SHOTS_INSIDE_BOX, src.IsHome))
            .Map(dest => dest.BigChances, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.BIG_CHANCES, src.IsHome))
            .Map(dest => dest.BigChancesMissed, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.BIG_CHANCES_MISSED, src.IsHome))
            .Map(dest => dest.Corners, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.CORNERS, src.IsHome))
            .Map(dest => dest.YellowCards, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.YELLOW_CARDS, src.IsHome))
            .Map(dest => dest.RedCards, src => GetFotmobFixtureSubSubStatValue<int>(src.TopStatsByKey, FotmobKeys.SubStats.RED_CARDS, src.IsHome))
            .Map(dest => dest.LineUp, src => new FixtureTeamDetailsLineUpMappingSource(
                src.LineUp,
                src.IsHome ? src.Fixture.HomeTeam.FotmobTeamId : src.Fixture.AwayTeam.FotmobTeamId,
                src.Team, src.TeamPlayers).Adapt<FixtureTeamDetailsLineUp>());

        // => FixtureTeamDetailsLineUp
        config.ForType<FixtureTeamDetailsLineUpMappingSource, FixtureTeamDetailsLineUp>()
            .MapToConstructor(true)
            .IgnoreNullValues(true)
            .Map(dest => dest.StartingPlayers, src => 
                src.LineUp!.StartingPlayers
                    .SelectMany(player => player)
                    .Select(player => new FixtureTeamPlayerDetailsMappingSource(player, player.FotmobPlayerName.FullName.GetFantasyPlayerFromFotmob(src.TeamPlayers, src.Team)))
                    .Adapt<IReadOnlyList<FixtureTeamPlayerDetails>>())
            .Map(dest => dest.BenchPlayers, src =>
                src.LineUp!.BenchPlayers
                    .Select(player => new FixtureTeamPlayerDetailsMappingSource(player, player.FotmobPlayerName.FullName.GetFantasyPlayerFromFotmob(src.TeamPlayers, src.Team)))
                    .Adapt<IReadOnlyList<FixtureTeamPlayerDetails>>());

        // => FixtureTeamPlayerDetails
        config.ForType<FixtureTeamPlayerDetailsMappingSource, FixtureTeamPlayerDetails>()
            .MapToConstructor(true)
            .IgnoreNullValues(true)
            .Map(dest => dest.PlayerId, src => src.FantasyPlayer!.Id)
            .Map(dest => dest.FotmobPlayerId, src => src.FotmobPlayer.FotmobPlayerId)
            .Map(dest => dest.DisplayName, src => src.FantasyPlayer!.DisplayName)
            .Map(dest => dest.MinutesPlayed, src => src.FotmobPlayer.MinutesPlayed)
            .Map(dest => dest.FotmobRating, src => GetFotmobLineUpPlayerStatValue<decimal?>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.FOTMOB_RATING))
            .Map(dest => dest.Goals, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.GOALS))
            .Map(dest => dest.Assists, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.ASSISTS))
            .Map(dest => dest.TotalShots, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.SHOTS))
            .Map(dest => dest.ChancesCreated, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.CHANCES_CREATED))
            .Map(dest => dest.ExpectedGoals, src => GetFotmobLineUpPlayerStatValue<string>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.EXPECTED_GOALS).ToDecimal(decimal.Zero))
            .Map(dest => dest.ExpectedAssists, src => GetFotmobLineUpPlayerStatValue<string>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.TOP_STATS,
                FotmobKeys.SubStats.EXPECTED_ASSISTS).ToDecimal(decimal.Zero))
            .Map(dest => dest.Clearances, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.DEFENCE,
                FotmobKeys.SubStats.CLEARANCES))
            .Map(dest => dest.Interceptions, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.DEFENCE,
                FotmobKeys.SubStats.INTERCEPTIONS))
            .Map(dest => dest.Recoveries, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.DEFENCE,
                FotmobKeys.SubStats.RECOVERIES))
            .Map(dest => dest.Saves, src => GetFotmobLineUpPlayerStatValue<int>(
                src.FotmobPlayer.Stats,
                FotmobKeys.LineUpPlayerStatType.DEFENCE,
                FotmobKeys.SubStats.SAVES));
            
    }

    private static T? GetFotmobFixtureSubSubStatValue<T>(Dictionary<string, FotmobFixtureDetailsSubStat>? statsByKey, string key, bool isHome)
    {
        if (statsByKey?.TryGetValue(key, out FotmobFixtureDetailsSubStat? subStat) ?? false)
        {
            return isHome
                ? subStat.SubSubStats.FirstOrDefault().GetJsonElementValue<T>()
                : subStat.SubSubStats.LastOrDefault().GetJsonElementValue<T>();
        }

        return default;
    }

    private static T? GetFotmobLineUpPlayerStatValue<T>(
        IReadOnlyList<FotmobLineUpPlayerStat> stats, 
        [ConstantExpected]string statWrapperName, 
        [ConstantExpected]string subStatName)
    {
        if (stats is not null && stats.FirstOrDefault(stat => stat.Title == statWrapperName) is { } statWrapper)
        {
            object? stat = statWrapper.Stats
                .GetType()
                .GetProperties()
                .Where(prop => prop.PropertyType == typeof(FotmobStatKeyValuePair))
                .Select(prop => prop.GetValue(statWrapper.Stats))
                .Where(obj => obj is not null)
                .FirstOrDefault(obj => ((FotmobStatKeyValuePair)obj!).Key == subStatName);

            return stat is FotmobStatKeyValuePair s && s.Value is JsonElement e
                ? e.GetJsonElementValue<T>()
                : default;
        }

        return default;
    }
}

internal static class CustomFotmobMapper
{
    internal static IReadOnlyList<FotmobPlayerDetails> ToPlayerDetails(
        this ILookup<(string, string), KeyValuePair<string, FotmobStat>> playerStatsByName, 
        List<Player> players, 
        IReadOnlyDictionary<string, Team> teamsByName)
    {
        List<FotmobPlayerDetails> playerDetails = [];
        foreach (IGrouping<(string PlayerName, string TeamName), KeyValuePair<string, FotmobStat>> stats in playerStatsByName)
        {
            Dictionary<string, FotmobStat> playerStats = stats.ToDictionary(kv => kv.Key, kv => kv.Value);

            string commonTeamName = stats.Key.TeamName.ToCommonTeamName();
            if (!teamsByName.ContainsKey(commonTeamName))
            {
                continue;
            }

            Player? fantasyPlayer = stats.Key.PlayerName.GetFantasyPlayerFromFotmob(players, teamsByName[commonTeamName]);
            if (fantasyPlayer is null)
            {
                continue;
            }

            FotmobPlayerDetails player = new(fantasyPlayer.Id, stats.Key.PlayerName, stats.Key.TeamName);
            players.Remove(fantasyPlayer);

            foreach (PropertyInfo property in player.GetType().GetProperties().Where(prop => !prop.IsInitOnly()))
            {
                if (playerStats.TryGetValue(property.Name, out FotmobStat? stat))
                {
                    property.SetValue(player, stat.StatValue);
                }
            }

            playerDetails.Add(player);
        }

        return playerDetails;
    }

    record PlayerMatchMap(int PlayerId, string[] PlayerNames);
    internal static Player? GetFantasyPlayerFromFotmob(this string fotmobPlayerName, IReadOnlyList<Player> players, Team team)
    {
        Dictionary<int, Player> playersById = players.ToDictionary(p => p.Id);

        string[] allNames = fotmobPlayerName
            .NormalizeDiacritics()
            .Split('-', ' ');

        List<PlayerMatchMap> playerFullNameMap = players
            .Where(player => player.TeamId == team.Id)
            .Select(player =>
                new PlayerMatchMap(player.Id, player.FullName!
                    .NormalizeDiacritics()
                    .Split('-', ' ')))
            .ToList();

        List<PlayerMatchMap> playerDisplayNameNameMap = players
            .Where(player => player.TeamId == team.Id)
            .Select(player =>
                new PlayerMatchMap(player.Id, player.DisplayName!
                    .NormalizeDiacritics()
                    .Split('-', ' ')))
            .ToList();

        bool TryMatch(List<PlayerMatchMap> map, Predicate<PlayerMatchMap> predicate, [NotNullWhen(true)] out Player? player)
        {
            PlayerMatchMap[] matches = map.Where(x => predicate(x)).ToArray();
            if (matches.Length == 1)
            {
                map.Remove(matches[0]);
                player = playersById[matches[0].PlayerId];
                return true;
            }

            player = null;
            return false;
        }

        foreach (string name in allNames)
        {
            // Check player full name
            if (TryMatch(playerFullNameMap, p => p.PlayerNames.Contains(name), out Player? p1))
            {
                return p1;
            }

            // Check player display name
            if (TryMatch(playerDisplayNameNameMap, p => p.PlayerNames.Contains(name), out Player? p2))
            {
                return p2;
            }

            // Check part of player display name
            if (TryMatch(playerDisplayNameNameMap, p => p.PlayerNames.Any(pName => pName.Contains(name)), out Player? p3))
            {
                return p3;
            }

            // Check part of player full name
            if (TryMatch(playerFullNameMap, p => p.PlayerNames.Any(pName => pName.Contains(name)), out Player? p4))
            {
                return p4;
            }
        }

        return null;
        //throw new MappingException($"Failed to map Fotmob player with name {fotmobPlayerName} in team {team.Name}");
    }

    internal static FixtureDetails ToFixtureDetails(
        this FotmobFixtureDetailsRoot fixtureDetailsRoot, 
        Fixture fixture, 
        Team homeTeam, Team awayTeam,
        IReadOnlyList<Player> HomeTeamPlayers,
        IReadOnlyList<Player> AwayTeamPlayers)
    {
        // Create necessary lookups,
        // Note that these will throw KeyNotFoundExceptions if the required main stats are not set up
        Dictionary<string, FotmobFixtureDetailsMainStat>? mainStatsByKey = 
            fixtureDetailsRoot.Stats.StatsRoot?.Periods.FullGame.MainStats?.ToDictionary(mainStat => mainStat.Key);
        Dictionary<string, FotmobFixtureDetailsSubStat>? topStatsByKey =
            mainStatsByKey?[FotmobKeys.MainStats.TOP_STATS].SubStats.ToDictionary(ss => ss.Key);
        Dictionary<string, FotmobFixtureDetailsSubStat>? shotStatsByKey =
            mainStatsByKey?[FotmobKeys.MainStats.SHOTS].SubStats.ToDictionary(ss => ss.Key);
        Dictionary<string, FotmobFixtureDetailsSubStat>? disciplineStatsByKey =
            mainStatsByKey?[FotmobKeys.MainStats.DISCIPLINE].SubStats.ToDictionary(ss => ss.Key);

        return new FixtureDetails(
            fixture.Id,
            fixtureDetailsRoot.Fixture.FotmobLeagueId,
            fixtureDetailsRoot.Fixture.FotmobFixtureId,
            fixture.GameweekId ?? -1,
            fixtureDetailsRoot.Fixture.KickOffTime,
            new FixtureTeamDetailsMappingSource(homeTeam, true, fixtureDetailsRoot.Fixture, topStatsByKey, shotStatsByKey, fixtureDetailsRoot.Stats.LineUpsRoot?.LineUps[0], HomeTeamPlayers)
                .Adapt<FixtureTeamDetails>(),
            new FixtureTeamDetailsMappingSource(awayTeam, false, fixtureDetailsRoot.Fixture, topStatsByKey, shotStatsByKey, fixtureDetailsRoot.Stats.LineUpsRoot?.LineUps[1], AwayTeamPlayers)
                .Adapt<FixtureTeamDetails>());
    }

    internal static string ToCommonTeamName(this string fotmobTeamName)
        => fotmobTeamName switch
        {
            "Häcken" => TeamNames.HACKEN,
            "Sirius" => TeamNames.SIRIUS,
            "Mjällby" => TeamNames.MJALLBY,
            "Degerfors" => TeamNames.DEGERFORS,
            "Varbergs BoIS FC" => TeamNames.VARBERG,
            "Brommapojkarna" => TeamNames.BROMMAPOJKARNA,
            "Elfsborg" => TeamNames.ELFSBORG,
            "West Ham United" => TeamNames.WEST_HAM,
            "Nottingham Forest" => TeamNames.NOTTINGHAM_FOREST,
            "Wolverhampton Wanderers" => TeamNames.WOLVERHAMPTHON,
            "Brighton & Hove Albion"
            or "Brighton and Hove Albion" => TeamNames.BRIGHTON,
            "Leeds United" => TeamNames.LEEDS,
            "AFC Bournemouth" => TeamNames.BOURNEMOUTH,
            "Manchester City" => TeamNames.MAN_CITY,
            "Manchester United" => TeamNames.MAN_UTD,
            "Newcastle United" => TeamNames.NEWCASTLE,
            "Tottenham Hotspur" => TeamNames.TOTTENHAM,
            "Leicester City" => TeamNames.LEICESTER,
            "Luton Town" => TeamNames.LUTON,
            "Sheffield United" => TeamNames.SHEFFIELD_UTD,
            _ => fotmobTeamName
        };
}

internal sealed class FotmobKeys
{
    internal sealed partial class MainStats
    {
        internal const string TOP_STATS = "top_stats";
        internal const string SHOTS = "shots";
        internal const string EXPECTED_GOALS = "expected_goals";
        internal const string PASSES = "passes";
        internal const string DEFENCE = "defence";
        internal const string DUELS = "duels";
        internal const string DISCIPLINE = "discipline";
    }

    internal sealed partial class SubStats
    {
        internal const string POSSESSION = "BallPossesion";
        internal const string EXPECTED_GOALS = "expected_goals";
        internal const string SHOTS = "total_shots";
        internal const string BIG_CHANCES = "big_chance";
        internal const string BIG_CHANCES_MISSED = "big_chance_missed_title";
        internal const string ACCURATE_PASSES = "accurate_passes";
        internal const string FOULS = "fouls";
        internal const string OFFSIDES = "Offsides";
        internal const string CORNERS = "corners";
        internal const string SHOTS_OFF_TARGET = "shotsOffTarget";
        internal const string SHOTS_ON_TARGET = "ShotsOnTarget";
        internal const string BLOCKED_SHOTS = "blocked_shots";
        internal const string SHOTS_IN_WOODWORK = "shots_woodwork";
        internal const string SHOTS_INSIDE_BOX = "shots_inside_box";
        internal const string SHOTS_OUTSIDE_BOX = "shots_outside_box";
        internal const string EXPECTED_GOALS_FIRST_HALF = "expected_goals_first_half";
        internal const string EXPECTED_GOALS_SECOND_HALF = "expected_goals_second_half";
        internal const string EXPECTED_GOALS_OPEN_PLAY = "expected_goals_open_play";
        internal const string EXPECTED_GOALS_SET_PLAY = "expected_goals_set_play";
        internal const string EXPECTED_GOALS_PENALTY = "expected_goals_penalty";
        internal const string EXPECTED_GOALS_ON_TARGET = "expected_goals_on_target";
        internal const string TOTAL_PASSES = "passes";
        internal const string OWN_HALF_PASSES = "own_half_passes";
        internal const string OPPOSITION_HALF_PASSES = "opposition_half_passes";
        internal const string ACCURATE_LONG_BALLS = "long_balls_accurate";
        internal const string ACCURATE_CROSSES = "accurate_crosses";
        internal const string TACKLES_WON = "tackles_succeeded";
        internal const string INTERCEPTIONS = "interceptions";
        internal const string CLEARANCES = "clearances";
        internal const string DUELS_WON = "duel_won";
        internal const string SUCCESSFUL_DRIBBLES = "dribbles_succeeded";
        internal const string YELLOW_CARDS = "yellow_cards";
        internal const string RED_CARDS = "red_cards";
        internal const string FOTMOB_RATING = "rating_title";
        internal const string GOALS = "goals";
        internal const string ASSISTS = "assists";
        internal const string CHANCES_CREATED = "chances_created";
        internal const string EXPECTED_ASSISTS = "expected_assists";
        internal const string RECOVERIES = "recoveries";
        internal const string SAVES = "saves";
    }

    internal sealed partial class LineUpPlayerStatType
    {
        internal const string TOP_STATS = "Top stats";
        internal const string ATTACK = "Attack";
        internal const string DEFENCE = "Defence";
        internal const string DUELS = "Duels";
    }
}