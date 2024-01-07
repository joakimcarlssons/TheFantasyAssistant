using TFA.Application.Common.Data;
using TFA.Application.Services.BaseData;

namespace TFA.Application.Features.Transforms;

public static class TeamTransforms
{
    /// <summary>
    /// Extract teams ranked by the total fixture difficulty within a given range of gameweeks.
    /// </summary>
    public static IReadOnlyList<TTeamToTarget> GetTeamsOrderedByFixtureDifficulty<TTeamOpponent, TTeamToTarget>(
        KeyedBaseData
        fantasyData,
        int fromGameweekId,
        int toGameweekId,
        int numberOfTeams,
        Func<KeyedBaseData, Team, Fixture, bool, TTeamOpponent> teamOpponentMapper,
        Func<Team, IReadOnlyList<TTeamOpponent>, int, TTeamToTarget> teamToTargetMapper)
        where TTeamOpponent : SummaryTeamOpponent
        where TTeamToTarget : SummaryTeamToTarget
    {
        IReadOnlyList<TTeamOpponent> AddOpponents(Team team, IEnumerable<Fixture> fixtures, bool isDouble)
            => fixtures
                .Select(fixture => teamOpponentMapper.Invoke(fantasyData, team, fixture, isDouble))
                .ToList();

        IReadOnlyList<Fixture> fixtures = fantasyData.FixturesByGameweekId
            .Where(f => f.Key >= fromGameweekId && f.Key <= toGameweekId)
            .SelectMany(group => group.Select(fixture => fixture))
            .ToList();

        return fantasyData.TeamsById.Values.Select(team =>
        {
            IReadOnlyList<Fixture> teamFixtures = fixtures
                .Where(fixture => fixture.HomeTeamId == team.Id || fixture.AwayTeamId == team.Id)
                .ToList();

            IReadOnlySet<int?> doubleGameweekIds = teamFixtures.GroupBy(fixture => fixture.GameweekId)
                .Where(gw => gw.Skip(1).Any())
                .Select(gw => gw.Key)
                .ToHashSet();

            IReadOnlyList<Fixture> doubleGameweekFixtures = teamFixtures
                .Where(f => doubleGameweekIds.Contains(f.GameweekId))
                .ToList();

            IReadOnlyList<TTeamOpponent> opponents =
                AddOpponents(team, doubleGameweekFixtures, true).Concat(
                    AddOpponents(team, teamFixtures.Except(doubleGameweekFixtures), false))
                .ToList();

            int blankGameweeks = (toGameweekId - fromGameweekId) - fixtures.Count;
            return teamToTargetMapper.Invoke(team, opponents, blankGameweeks);
        })
            .OrderByDescending(team => team.NumberOfOpponents)
            .ThenBy(team => team.TotalDifficulty)
            .ThenBy(team => team.Position)
            .Take(numberOfTeams)
            .ToList();
    }
}
