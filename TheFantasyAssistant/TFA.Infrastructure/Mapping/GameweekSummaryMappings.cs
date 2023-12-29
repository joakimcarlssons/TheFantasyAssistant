using TFA.Application.Features.GameweekFinished;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Player;

namespace TFA.Infrastructure.Mapping;

public class GameweekSummaryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<(
            Player Player,
            Team Team,
            IReadOnlyList<FixtureDetails> FotmobFixtureDetails,
            IReadOnlyList<FantasyPlayerFixtureHistoryRequest> FantasyFixtureDetails), GameweekSummaryPlayer>()
            .MapToConstructor(true)
            .Map(src => src.PlayerId, dest => dest.Player.Id)
            .Map(src => src.DisplayName, dest => dest.Player.DisplayName)
            .Map(src => src.TeamShortName, dest => dest.Team.ShortName)
            .Map(src => src.Points, dest => dest.FantasyFixtureDetails.Sum(x => x.Points))
            .Map(src => src.Position, dest => dest.Player.Position)
            .Map(src => src.Opponents, dest => GetPlayerOpponents(dest.FotmobFixtureDetails, dest.Player.TeamId))
            .Map(src => src.MinutesPlayed, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.MinutesPlayed))
            .Map(src => src.Goals, dest => dest.FantasyFixtureDetails.Sum(x => x.Goals))
            .Map(src => src.Assists, dest => dest.FantasyFixtureDetails.Sum(x => x.Assists))
            .Map(src => src.TotalShots, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.TotalShots))
            .Map(src => src.ChancesCreated, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.ChancesCreated))
            .Map(src => src.ExpectedGoals, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.ExpectedGoals))
            .Map(src => src.ExpectedAssists, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.ExpectedAssists))
            .Map(src => src.Clearances, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.Clearances))
            .Map(src => src.Interceptions, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.Interceptions))
            .Map(src => src.Recoveries, dest => GetFotmobFixturePlayerDetails(dest.FotmobFixtureDetails, dest.Player).Sum(x => x.Recoveries))
            .Map(src => src.Saves, dest => dest.FantasyFixtureDetails.Sum(x => x.Saves))
            .Map(src => src.CleanSheets, dest => dest.FantasyFixtureDetails.Sum(x => x.CleanSheets))
            .Map(src => src.AttackingBonus, dest => dest.FantasyFixtureDetails.Sum(x => x.AttackingBonus))
            .Map(src => src.DefendingBonus, dest => dest.FantasyFixtureDetails.Sum(x => x.DefendingBonus))
            .Map(src => src.WinningGoals, dest => dest.FantasyFixtureDetails.Sum(x => x.WinningGoals))
            .Map(src => src.KeyPasses, dest => dest.FantasyFixtureDetails.Sum(x => x.KeyPasses))
            .Map(src => src.Bonus, dest => dest.FantasyFixtureDetails.Sum(x => x.Bonus))
            .Map(src => src.Bps, dest => dest.FantasyFixtureDetails.Sum(x => x.Bps));
    }

    private static IReadOnlyList<GameweekSummaryPlayerOpponent> GetPlayerOpponents(IReadOnlyList<FixtureDetails> fixtures, int teamId)
        => fixtures
            .Where(fixture => fixture.HomeTeam is { TeamId: > 0 } && fixture.HomeTeam?.TeamId == teamId)
            .Select(fixture => (fixture.HomeTeam, true))
            .Concat(fixtures
                .Where(fixture => fixture.AwayTeam is { TeamId: > 0 } && fixture.AwayTeam?.TeamId == teamId)
                .Select(fixture => (fixture.HomeTeam, false)))
            .Select(team => new GameweekSummaryPlayerOpponent(team.HomeTeam!.TeamShortName, team.Item2))
            .ToList();

    private static IReadOnlyList<FixtureTeamPlayerDetails> GetFotmobFixturePlayerDetails(IReadOnlyList<FixtureDetails> fotmobFixtureDetails, Player player)
        => fotmobFixtureDetails
            .Where(fixture => fixture.HomeTeam is not null && fixture.AwayTeam is not null)
            .Where(fixture => fixture.HomeTeam!.TeamId == player.TeamId || fixture.AwayTeam!.TeamId == player.TeamId)
            .SelectMany(fixture =>
                fixture.HomeTeam!.LineUp.BenchPlayers
                .Concat(fixture.HomeTeam!.LineUp.StartingPlayers)
                .Concat(fixture.AwayTeam!.LineUp.BenchPlayers
                .Concat(fixture.AwayTeam!.LineUp.StartingPlayers)))
            .Where(fotmobPlayer => fotmobPlayer.PlayerId == player.Id)
            .ToList();
}
