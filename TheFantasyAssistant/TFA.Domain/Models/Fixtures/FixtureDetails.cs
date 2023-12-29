namespace TFA.Domain.Models.Fixtures;

public sealed record FixtureDetails(
    [property: JsonPropertyName("fixture_id")] int FixtureId,
    [property: JsonPropertyName("fotmob_league_id")] int FotmobLeagueId,
    [property: JsonPropertyName("fotmob_fixture_id")] string FotmobFixtureId,
    [property: JsonPropertyName("gameweek")] int Gameweek,
    [property: JsonPropertyName("kick_off_time")] DateTime? KickOffTime,
    [property: JsonPropertyName("home_team")] FixtureTeamDetails? HomeTeam,
    [property: JsonPropertyName("away_team")] FixtureTeamDetails? AwayTeam);

public sealed record FixtureTeamDetails(
    [property: JsonPropertyName("team_id")] int TeamId,
    [property: JsonPropertyName("fotmob_team_id")] int FotmobTeamId,
    [property: JsonPropertyName("team_name")] string TeamName,
    [property: JsonPropertyName("team_short_name")] string TeamShortName,
    [property: JsonPropertyName("ball_possession")] int Possession,
    [property: JsonPropertyName("expected_goals")] decimal ExpectedGoals,
    [property: JsonPropertyName("total_shots")] int TotalShots,
    [property: JsonPropertyName("shots_off_target")] int ShotsOffTarget,
    [property: JsonPropertyName("shots_on_target")] int ShotsOnTarget,
    [property: JsonPropertyName("shots_inside_box")] int ShotsInsideBox,
    [property: JsonPropertyName("big_chances")] int BigChances,
    [property: JsonPropertyName("big_chances_missed")] int BigChancesMissed,
    [property: JsonPropertyName("corners")] int Corners,
    [property: JsonPropertyName("yellow_cards")] int YellowCards,
    [property: JsonPropertyName("red_cards")] int RedCards,
    [property: JsonPropertyName("line_up")] FixtureTeamDetailsLineUp LineUp);

public sealed record FixtureTeamDetailsLineUp(
    [property: JsonPropertyName("starting_players")] IReadOnlyList<FixtureTeamPlayerDetails> StartingPlayers,
    [property: JsonPropertyName("bench_players")] IReadOnlyList<FixtureTeamPlayerDetails> BenchPlayers);

/// <param name="PlayerId">The fantasy player id.</param>
public sealed record FixtureTeamPlayerDetails(
    [property: JsonPropertyName("player_id")] int PlayerId,
    [property: JsonPropertyName("fotmob_player_id")] string FotmobPlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("minutes_played")] int MinutesPlayed,
    [property: JsonPropertyName("fotmob_rating")] decimal? FotmobRating,
    [property: JsonPropertyName("goals_scored")] int Goals,
    [property: JsonPropertyName("assists")] int Assists,
    [property: JsonPropertyName("total_shots")] int TotalShots,
    [property: JsonPropertyName("chances_created")] int ChancesCreated,
    [property: JsonPropertyName("expected_goals")] decimal ExpectedGoals,
    [property: JsonPropertyName("expected_assists")] decimal ExpectedAssists,
    [property: JsonPropertyName("clearances")] int Clearances,
    [property: JsonPropertyName("interceptions")] int Interceptions,
    [property: JsonPropertyName("recoveries")] int Recoveries,
    [property: JsonPropertyName("saves")] int Saves);