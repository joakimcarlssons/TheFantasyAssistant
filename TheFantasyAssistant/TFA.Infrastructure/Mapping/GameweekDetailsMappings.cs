using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.FixtureLiveUpdate;
using TFA.Application.Features.GameweekLiveUpdate.Events;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Gameweek;

namespace TFA.Infrastructure.Mapping;


public class GameweekDetailsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<(
            int Gameweek,
            IReadOnlyList<Fixture> Fixtures,
            IReadOnlyDictionary<int, Team> TeamsById, 
            ILookup<int, FantasyGameweekLivePlayerRequest> GameweekLivePlayersByFixtureId,
            ILookup<int, Player> PlayersByTeamId), GameweekLiveUpdateData>()
            .MapToConstructor(true)
            .Map(dest => dest.Gameweek, src => src.Gameweek)
            .Map(dest => dest.FinishedFixtures, src => MapFinishedFixtures(src.Fixtures, src.TeamsById, src.GameweekLivePlayersByFixtureId, src.PlayersByTeamId));

        config.ForType<(
            Player Player,
            FantasyGameweekLivePlayerDetailsRequest FixtureDetails), GameweekLiveFinishedFixturePlayerDetails>()
            .MapToConstructor(true)
            .Map(d => d.PlayerId, s => s.Player.Id)
            .Map(d => d.DisplayName, s => s.Player.DisplayName)
            .Map(d => d.TotalPoints, s => s.FixtureDetails.FixtureStats.Sum(x => x.Points))
            .Map(d => d.MinutesPlayed, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.MinutesPlayed))
            .Map(d => d.GoalsScored, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.Goals))
            .Map(d => d.Assists, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.Assists))
            .Map(d => d.CleanSheets, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.CleanSheets))
            .Map(d => d.AttackingBonus, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.AttackingBonus))
            .Map(d => d.DefendingBonus, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.DefendingBonus))
            .Map(d => d.Bonus, s => GetFixtureStatValue(s.FixtureDetails.FixtureStats, FixtureStatIdentifiers.Bonus));

        config.ForType<GameweekLiveUpdateData, GameweekLiveUpdatePresentModel>()
            .MapToConstructor(true)
            .Map(d => d.Gameweek, s => s.Gameweek);

        config.ForType<GameweekLiveFinishedFixture, GameweekLiveFixtureFinishedPresentModel>()
            .MapToConstructor(true)
            .Map(d => d.FixtureId, s => s.FixtureId)
            .Map(d => d.HomeTeamName, s => s.HomeTeam.TeamName)
            .Map(d => d.HomeTeamShortName, s => s.HomeTeam.TeamShortName)
            .Map(d => d.AwayTeamName, s => s.AwayTeam.TeamName)
            .Map(d => d.AwayTeamShortName, s => s.AwayTeam.TeamShortName)
            .Map(d => d.HomeTeamScore, s => s.HomeTeam.GoalsScored)
            .Map(d => d.AwayTeamScore, s => s.AwayTeam.GoalsScored)
            
            // Goal scorers
            .Map(d => d.GoalScorers, s => s.HomeTeam.Players
                .Where(p => p.GoalsScored > 0)
                .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.HomeTeam.TeamShortName, p.GoalsScored)
                .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>())
                .Concat(s.AwayTeam.Players
                    .Where(p => p.GoalsScored > 0)
                    .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.AwayTeam.TeamShortName, p.GoalsScored)
                    .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>()))
                .OrderByDescending(p => p.Value))

            // Assisters
            .Map(d => d.Assisters, s => s.HomeTeam.Players
                .Where(p => p.Assists > 0)
                .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.HomeTeam.TeamShortName, p.Assists)
                .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>())
                .Concat(s.AwayTeam.Players
                    .Where(p => p.Assists > 0)
                    .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.AwayTeam.TeamShortName, p.Assists)
                    .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>()))
                .OrderByDescending(p => p.Value))

            // Bonus
            .Map(d => d.BonusPlayers, s => s.HomeTeam.Players
                .Where(p => p.Bonus > 0)
                .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.HomeTeam.TeamShortName, p.Bonus)
                .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>())
                .Concat(s.AwayTeam.Players
                    .Where(p => p.Bonus > 0)
                    .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.AwayTeam.TeamShortName, p.Bonus)
                    .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>()))
                .OrderByDescending(p => p.Value))

            // Att. Bonus
            .Map(d => d.AttackingBonusPlayers, s => s.HomeTeam.Players
                .Where(p => p.AttackingBonus > 0)
                .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.HomeTeam.TeamShortName, p.AttackingBonus)
                .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>())
                .Concat(s.AwayTeam.Players
                    .Where(p => p.AttackingBonus > 0)
                    .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.AwayTeam.TeamShortName, p.AttackingBonus)
                    .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>()))
                .OrderByDescending(p => p.Value))

            // Def. Bonus
            .Map(d => d.DefendingBonusPlayers, s => s.HomeTeam.Players
                .Where(p => p.DefendingBonus > 0)
                .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.HomeTeam.TeamShortName, p.DefendingBonus)
                .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>())
                .Concat(s.AwayTeam.Players
                    .Where(p => p.DefendingBonus > 0)
                    .Select(p => new FinishedFixturePlayerPresentModelMapper(p, s.AwayTeam.TeamShortName, p.DefendingBonus)
                    .Adapt<GameweekLiveFixtureFinishedPlayerPresentModel>()))
                .OrderByDescending(p => p.Value));

        config.ForType<FinishedFixturePlayerPresentModelMapper, GameweekLiveFixtureFinishedPlayerPresentModel>()
            .MapToConstructor(true)
            .Map(d => d.PlayerId, s => s.Player.PlayerId)
            .Map(d => d.TeamShortName, s => s.TeamShortName)
            .Map(d => d.DisplayName, s => s.Player.DisplayName)
            .Map(d => d.Value, s => s.Value);
    }

    private static IReadOnlyList<GameweekLiveFinishedFixture> MapFinishedFixtures(
        IReadOnlyList<Fixture> fixtures,
        IReadOnlyDictionary<int, Team> TeamsById,
        ILookup<int, FantasyGameweekLivePlayerRequest> gameweekLivePlayersByFixtureId,
        ILookup<int, Player> playersByTeamId)
    {
        return fixtures.Select(fixture =>
        {
            IReadOnlyDictionary<int, Player> homeTeamPlayersById = playersByTeamId[fixture.HomeTeamId].ToDictionary(p => p.Id);
            IReadOnlyDictionary<int, Player> awayTeamPlayersById = playersByTeamId[fixture.AwayTeamId].ToDictionary(p => p.Id);
            Team homeTeam = TeamsById[fixture.HomeTeamId];
            Team awayTeam = TeamsById[fixture.AwayTeamId];

            IReadOnlyList<GameweekLiveFinishedFixturePlayerDetails?> mappedHomeTeamPlayers = gameweekLivePlayersByFixtureId[fixture.Id]
                .Select(player =>
                {
                    if(player.GameweekDetails?.FirstOrDefault(details => details.FixtureId == fixture.Id) is { } details
                       && homeTeamPlayersById.TryGetValue(player.PlayerId, out Player? homeTeamPlayer))
                    {
                        return (homeTeamPlayer, details).Adapt<GameweekLiveFinishedFixturePlayerDetails>();
                    }

                    return null;                    
                })
                .Where(x => x is not null)
                .ToList();

            IReadOnlyList<GameweekLiveFinishedFixturePlayerDetails?> mappedAwayTeamPlayers = gameweekLivePlayersByFixtureId[fixture.Id]
                .Select(player =>
                {
                    if (player.GameweekDetails?.FirstOrDefault(details => details.FixtureId == fixture.Id) is { } details
                        && awayTeamPlayersById.TryGetValue(player.PlayerId, out Player? awayTeamPlayer))
                    {
                        return (awayTeamPlayer, details).Adapt<GameweekLiveFinishedFixturePlayerDetails>();
                    }

                    return null;
                })
                .Where(x => x is not null)
                .ToList();

            return new GameweekLiveFinishedFixture(
                fixture.Id,
                new GameweekLiveFinishedFixtureTeamDetails(homeTeam.Id, homeTeam.Name, homeTeam.ShortName, fixture.HomeTeamScore ?? 0, mappedHomeTeamPlayers!),
                new GameweekLiveFinishedFixtureTeamDetails(awayTeam.Id, awayTeam.Name, awayTeam.ShortName, fixture.AwayTeamScore ?? 0, mappedAwayTeamPlayers!));
        }).ToList();
    }

    /// <summary>
    /// Get a fixture stat value by providing a identifier key from <see cref="FixtureStatIdentifiers"/>
    /// </summary>
    /// <param name="identifier">The key to identify the value with</param>
    private static int GetFixtureStatValue(IEnumerable<FantasyGameweekLivePlayerDetailStatRequest> stats, [ConstantExpected] string identifier)
        => stats.FirstOrDefault(s => s.StatType == identifier)?.Value ?? 0;

}

internal readonly record struct FinishedFixturePlayerPresentModelMapper(GameweekLiveFinishedFixturePlayerDetails Player, string TeamShortName, int Value);