using System.Text.Json;

namespace TFA.Infrastructure.Dtos.Fotmob;

/// <summary>
/// Model used to map data such as Expected goals/assists etc.
/// </summary>
public sealed record FotmobTopListRoot(
    [property: JsonPropertyName("TopLists")] IReadOnlyList<FotmobTopList> TopLists);

public sealed record FotmobTopList(
    [property: JsonPropertyName("StatName")] string StatName,
    [property: JsonPropertyName("StatList")] IReadOnlyList<FotmobStat> Stats);

/// <summary>
/// Model of a stat coming from Fotmob for both teams and players.
/// </summary>
/// <param name="EntityName">The name of the player or team.</param>
/// <param name="FotmobPlayerId">The player id if the entity is a player. Else 0.</param>
/// <param name="FotmobTeamId">The team id if the entity is a team. Else 0.</param>
/// <param name="TeamName">The name of the team if the entity is a player. If the entity is a team the name is <paramref name="EntityName"/></param>
/// <param name="StatValue">The value of the stat being looked at.</param>
/// <param name="SubStatValue">The outcome of the checked value. E.g goals if checking xG or assists if looking at xA.</param>
/// <param name="TotalMinutesPlayed">Only set if the entity is a player.</param>
/// <param name="StatValueCount">The basis on which the result is based.</param>
public sealed record FotmobStat(
    [property: JsonPropertyName("ParticipantName")] string EntityName,
    [property: JsonPropertyName("ParticipantId")] int FotmobPlayerId,
    [property: JsonPropertyName("TeamId")] int FotmobTeamId,
    [property: JsonPropertyName("TeamName")] string? TeamName,
    [property: JsonPropertyName("StatValue")] double StatValue,
    [property: JsonPropertyName("SubStatValue")] double SubStatValue,
    [property: JsonPropertyName("MinutesPlayed")] int TotalMinutesPlayed,
    [property: JsonPropertyName("MatchesPlayed")] int MatchesPlayed,
    [property: JsonPropertyName("StatValueCount")] int StatValueCount,
    [property: JsonPropertyName("Rank")] int Rank)
{
    /// <summary>
    /// The internal name of stat. 
    /// </summary>
    public string? StatName { get; init; }
}

public sealed record FotmobLeagueRoot(
    [property: JsonPropertyName("matches")] FotmobLeagueFixtureWrapper? FixtureWrapper,
    [property: JsonPropertyName("table")] FotmobLeagueTableRequestRoot[] Tables);

public sealed record FotmobLeagueFixtureWrapper(
    [property: JsonPropertyName("allMatches")] IReadOnlyList<FotmobFixture>? Fixtures);

public sealed record FotmobFixture(
    [property: JsonPropertyName("round")] int Gameweek,
    [property: JsonPropertyName("id")] string FotmobFixtureId,
    [property: JsonPropertyName("home")] FotmobFixtureTeam HomeTeam,
    [property: JsonPropertyName("away")] FotmobFixtureTeam AwayTeam,
    [property: JsonPropertyName("status")] FotmobFixtureStatus FixtureStatus);

public sealed record FotmobFixtureTeam(
    [property: JsonPropertyName("id")] int FotmobTeamId,
    [property: JsonPropertyName("name")] string TeamName,
    [property: JsonPropertyName("shortName")] string TeamShortName);

public sealed record FotmobFixtureStatus(
    [property: JsonPropertyName("utcTime")] DateTime KickOffTime,
    [property: JsonPropertyName("finished")] bool IsFinished);

public sealed record FotmobFixtureDetailsRoot(
    [property: JsonPropertyName("general")] FotmobFixtureDetails Fixture,
    [property: JsonPropertyName("content")] FotmobFixtureDetailsStats Stats);

/// <param name="Gameweek">
/// This is the gameweek where the game should've taken place. 
/// In case of double gameweek where the fixture has been moved we need to look at KickOffTime.
/// </param>
public sealed record FotmobFixtureDetails(
    [property: JsonPropertyName("matchId")] string FotmobFixtureId,
    [property: JsonPropertyName("matchRound")] string Gameweek,
    [property: JsonPropertyName("leagueId")] int FotmobLeagueId,
    [property: JsonPropertyName("homeTeam")] FotmobFixtureTeam HomeTeam,
    [property: JsonPropertyName("awayTeam")] FotmobFixtureTeam AwayTeam,
    [property: JsonPropertyName("matchTimeUTCDate")] DateTime? KickOffTime);

public sealed record FotmobFixtureDetailsStats(
    [property: JsonPropertyName("stats")] FotmobFixtureDetailsStatsRoot? StatsRoot,
    [property: JsonPropertyName("lineup")] FotmobFixtureDetailsLineUpRoot? LineUpsRoot);

public sealed record FotmobFixtureDetailsStatsRoot(
    [property: JsonPropertyName("Periods")] FotmobFixtureDetailsStatsRootPeriod Periods);

public sealed record FotmobFixtureDetailsStatsRootPeriod(
    [property: JsonPropertyName("All")] FotmobFixtureDetailsStatsRootPeriodType FullGame,
    [property: JsonPropertyName("FirstHalf")] FotmobFixtureDetailsStatsRootPeriodType FirstHalf,
    [property: JsonPropertyName("SecondHalf")] FotmobFixtureDetailsStatsRootPeriodType SecondHalf);

public sealed record FotmobFixtureDetailsStatsRootPeriodType(
    [property: JsonPropertyName("stats")] IReadOnlyList<FotmobFixtureDetailsMainStat>? MainStats);

public sealed record FotmobFixtureDetailsMainStat(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("stats")] IReadOnlyList<FotmobFixtureDetailsSubStat> SubStats);

/// <param name="SubSubStats">
/// Contains stats for both the home and away team.
/// These stats are different depending on the key and type of stat.
/// The type of stat is described in the <paramref name="SubSubStatType"/>
/// </param>
/// <param name="SubStatWinner">
/// This is either home, away or equal.
/// Home team is always the first value in the <paramref name="SubSubStats"/> array.
/// </param>
public sealed record FotmobFixtureDetailsSubStat(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("stats")] JsonElement[] SubSubStats,
    [property: JsonPropertyName("type")] string SubSubStatType,
    [property: JsonPropertyName("highlighted")] string SubStatWinner);

public sealed record FotmobFixtureDetailsLineUpRoot(
    [property: JsonPropertyName("lineup")] IReadOnlyList<FotmobLineUp> LineUps);

/// <param name="StartingPlayers">Divided into arrays within the array for positions (GK, DEF, MID, ATT)</param>
public sealed record FotmobLineUp(
    [property: JsonPropertyName("teamId")] int FotmobTeamId,
    [property: JsonPropertyName("teamName")] string FotmobTeamName,
    [property: JsonPropertyName("bench")] IReadOnlyList<FotmobLineUpPlayer> BenchPlayers,
    [property: JsonPropertyName("players")] IReadOnlyList<IReadOnlyList<FotmobLineUpPlayer>> StartingPlayers);

public sealed record FotmobLineUpPlayer(
    [property: JsonPropertyName("id")] string FotmobPlayerId,
    [property: JsonPropertyName("name")] FotmobLineUpPlayerName FotmobPlayerName,
    [property: JsonPropertyName("minutesPlayed")] int MinutesPlayed,
    [property: JsonPropertyName("stats")] IReadOnlyList<FotmobLineUpPlayerStat> Stats);

public sealed record FotmobLineUpPlayerName(
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("fullName")] string FullName);

public sealed record FotmobLineUpPlayerStat(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("stats")] FotmobLineUpPlayerStatValuesWrapper Stats);

/// <summary>
/// Contains all type of stat values. 
/// This means that some values might be null because they're not part of the current stat title.
/// </summary>
public sealed record FotmobLineUpPlayerStatValuesWrapper(
    [property: JsonPropertyName("FotMob rating")] FotmobStatKeyValuePair? FotmobRating,
    [property: JsonPropertyName("Goals")] FotmobStatKeyValuePair? Goals,
    [property: JsonPropertyName("Assists")] FotmobStatKeyValuePair? Assists,
    [property: JsonPropertyName("Total shots")] FotmobStatKeyValuePair? Shots,
    [property: JsonPropertyName("Chances created")] FotmobStatKeyValuePair? ChancesCreated,
    [property: JsonPropertyName("Expected goals (xG)")] FotmobStatKeyValuePair? ExpectedGoals,
    [property: JsonPropertyName("Expected assists (xA)")] FotmobStatKeyValuePair? ExpectedAssists,
    [property: JsonPropertyName("Clearances")] FotmobStatKeyValuePair? Clearances,
    [property: JsonPropertyName("Interceptions")] FotmobStatKeyValuePair? Interceptions,
    [property: JsonPropertyName("Recoveries")] FotmobStatKeyValuePair? Recoveries,
    [property: JsonPropertyName("Saves")] FotmobStatKeyValuePair? Saves);

public sealed record FotmobStatKeyValuePair(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("value")] object Value);


/// <summary>
/// Coming in as an array.
/// </summary>
public sealed record FotmobLeagueTableRequestRoot(
    [property: JsonPropertyName("data")] FotmobLeagueTableData Data);

public sealed record FotmobLeagueTableData(
    [property: JsonPropertyName("leagueId")] int FotmobLeagueId,
    [property: JsonPropertyName("table")] FotmobLeagueTableRoot Table);

public sealed record FotmobLeagueTableRoot(
    [property: JsonPropertyName("all")] IReadOnlyList<FotmobLeagueTableTeam> Teams);

public sealed record FotmobLeagueTableTeam(
    [property: JsonPropertyName("name")] string FotmobTeamName,
    [property: JsonPropertyName("shortName")] string FotmobTeamShortName,
    [property: JsonPropertyName("id")] int FotmobTeamId,
    [property: JsonPropertyName("deduction")] int? PointsDeduction,
    [property: JsonPropertyName("played")] int MatchesPlayed,
    [property: JsonPropertyName("wins")] int Wins,
    [property: JsonPropertyName("draws")] int Draws,
    [property: JsonPropertyName("losses")] int Losses,
    [property: JsonPropertyName("scoresStr")] string GoalDifferenceStr,
    [property: JsonPropertyName("goalConDiff")] int GoalDifference,
    [property: JsonPropertyName("pts")] int Points,
    [property: JsonPropertyName("idx")] int Position,
    [property: JsonPropertyName("ongoing")] FotmobLeagueOngoingMatch? OngoingMatch);

public sealed record FotmobLeagueOngoingMatch(
    [property: JsonPropertyName("id")] long FotmobFixtureId,
    [property: JsonPropertyName("hTeam")] string HomeTeamName,
    [property: JsonPropertyName("aTeam")] string AwayTeamName,
    [property: JsonPropertyName("hScore")] int HomeTeamScore,
    [property: JsonPropertyName("aScore")] int AwayTeamScore,
    [property: JsonPropertyName("hId")] int HomeTeamFotmobId,
    [property: JsonPropertyName("aId")] int AwayTeamFotmobId);