using TFA.Application.Common.Data;
using TFA.Application.Common.Keys;
using TFA.Application.Errors;
using TFA.Application.Features.Deadline;
using TFA.Application.Features.Transforms;
using TFA.Application.Interfaces.Repositories;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Services.Common;

namespace TFA.Infrastructure.Services;

public sealed class DeadlineSummaryService(
    ILogger<DeadlineSummaryService> logger,
    IBaseDataService baseData,
    IFotmobService fotmob,
    IFirebaseRepository db) : AbstractSummaryService<DeadlineSummaryData>
{
    public override async Task<ErrorOr<DeadlineSummaryData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType, cancellationToken);
        if (fantasyData.IsError)
        {
            return fantasyData.ErrorsOrEmptyList;
        }

        Gameweek? nextGameweek = GetNextGameweek(fantasyData.Value.GameweeksById.Values);
        if (nextGameweek is null)
        {
            logger.LogCouldNotProceedBecauseOfInvalidData(typeof(DeadlineSummaryService), typeof(Gameweek));
            return Errors.Service.InvalidData;
        }

        if (!(await ShouldProceedWithDeadlineSummary(nextGameweek, fantasyType)))
        {
            logger.LogJobSkipped(typeof(DeadlineSummaryService), "Too far until next deadline.");
            return Errors.Service.Skipped;
        }

        if (fantasyType == FantasyType.Allsvenskan)
        {
            // Todo: Add custom point projections
        }

        IReadOnlyList<Player> playersToTarget = GetPlayersToTarget(fantasyData.Value.PlayersById.Values, fantasyType).Take(5).ToList();
        IReadOnlyDictionary<int, FotmobPlayerDetails> playerDetailsById = (await fotmob.GetFotmobPlayerDetails(
            fantasyData.Value with 
            {
                PlayersById = playersToTarget.ToDictionary(p => p.Id)
            },
            fantasyType, 
            cancellationToken)).ToDictionary(pd => pd.PlayerId);

        return new DeadlineSummaryData(
            fantasyType,
            nextGameweek,
            playersToTarget.Select(p =>
                (p, playerDetailsById[p.Id], fantasyData.Value.TeamsById[p.TeamId])
                .Adapt<DeadlineSummaryPlayerToTarget>())
            .ToList(),
            GetPlayersRiskingSuspension(fantasyData.Value, fantasyType),
            TeamTransforms.GetTeamsOrderedByFixtureDifficulty(
                fantasyData.Value, 
                nextGameweek.Id, 
                nextGameweek.Id,
                numberOfTeams: 4,
                MapOpponent,
                MapTeamToTarget),
            TeamTransforms.GetTeamsOrderedByFixtureDifficulty(
                fantasyData.Value, 
                nextGameweek.Id + 1, 
                nextGameweek.Id + 3,
                numberOfTeams: 3,
                MapOpponent,
                MapTeamToTarget));
    }

    /// <summary>
    /// Get the upcoming gameweek, if any.
    /// </summary>
    private static Gameweek? GetNextGameweek(IEnumerable<Gameweek> gameweeks)
        => gameweeks.FirstOrDefault(gw => gw.IsNext);

    /// <summary>
    /// Verify if we should move on to fetch and map data depending on the remaining time until the deadline.
    /// </summary>
    private async ValueTask<bool> ShouldProceedWithDeadlineSummary(Gameweek? gameweek, FantasyType fantasyType)
    {
        if (Env.IsDevelopment())
            return true;

        if (gameweek is null)
            return false;

        string latestCheckedDeadlineDataKey = DataKeysHandler.GetDataKey(fantasyType, KeyType.LastCheckedDeadline);
        int latestCheckedDeadline = await db.Get<int>(latestCheckedDeadlineDataKey);

        return gameweek.Deadline.AddDays(-1) < DateTime.UtcNow && latestCheckedDeadline < gameweek.Id;
    }

    /// <summary>
    /// Extract players to target.
    /// </summary>
    private static IEnumerable<Player> GetPlayersToTarget(IEnumerable<Player> players, FantasyType fantasyType)
    {
        foreach (Player player in players.Where(p =>
            p.Position != PlayerPosition.Goalkeeper
            && p.ChanceOfPlayingNextRound == 100
            && p.Status == PlayerStatuses.Available)
            .OrderByDescending(p => p.ExpectedPointsNextGameweek.ToDecimal())
            .ThenByDescending(p => p.Form.ToDecimal())
            .ThenByDescending(p => fantasyType switch
            {
                FantasyType.FPL => p.Bps,
                FantasyType.Allsvenskan => (p.AttackingBonus + p.DefendingBonus),
                _ => throw new FantasyTypeNotSupportedException()
            })
            .ThenBy(p => p.SelectedByPercent.ToDecimal()))
        {
            yield return player;
        }
    }

    /// <summary>
    /// Extract players risking suspension.
    /// Note that there are different rules depending on the <paramref name="fantasyType"/>
    /// </summary>
    private static IReadOnlyList<DeadlineSummaryPlayerRiskingSuspension> GetPlayersRiskingSuspension(KeyedBaseData fantasyData, FantasyType fantasyType)
    {
        return fantasyType switch
        {
            FantasyType.FPL => GetFPLPlayersRiskingSuspension(fantasyData.PlayersById.Values, fantasyData.TeamsById),
            FantasyType.Allsvenskan => [],
            _ => []
        };
    }

    private DeadlineSummaryTeamOpponent MapOpponent(KeyedBaseData fantasyData, Team team, Fixture fixture, bool isDouble)
    {
        bool isHome = fixture.HomeTeamId == team.Id;
        return new DeadlineSummaryTeamOpponent(
            fixture.Id,
            fixture.GameweekId ?? 1,
            GetOpponentShortName(fantasyData, isHome, fixture.HomeTeamId, fixture.AwayTeamId),
            isHome,
            isDouble,
            false,
            GetFixtureDifficulty(isHome, fixture));
    }

    private static DeadlineSummaryTeamToTarget MapTeamToTarget(Team team, IReadOnlyList<DeadlineSummaryTeamOpponent> opponents, int blankGameweeks)
        => new(
            team.Id,
            team.Name,
            team.ShortName,
            team.Position ?? 99,
            opponents.Sum(opp => opp.FixtureDifficulty) + Math.Max((blankGameweeks * 6), 0),
            opponents.OrderBy(opp => opp.Gameweek).ToList());

    private static int CalculateDoubleGameweekFixtureDifficulty(int baseFixtureDifficulty)
        => baseFixtureDifficulty switch
        {
            5 => 3,
            4 => 2,
            3 => 1,
            2 => 0,
            _ => 1
        };

    private static IReadOnlyList<DeadlineSummaryPlayerRiskingSuspension> GetFPLPlayersRiskingSuspension(IEnumerable<Player> players, IReadOnlyDictionary<int, Team> teamsById)
        => players.Where(player =>
        {
            if (teamsById.TryGetValue(player.TeamId, out Team? team))
            {
                return (team.MatchesPlayed < 19 && player.YellowCards == 4)
                    || (team.MatchesPlayed < 32 && player.YellowCards == 9)
                    || (team.MatchesPlayed < 38 && player.YellowCards == 14);
            }

            return false;
        })
        .Select(player => (player, teamsById[player.TeamId]).Adapt<DeadlineSummaryPlayerRiskingSuspension>())
        .ToList();
}
