using TFA.Application.Common.Extensions;
using TFA.Application.Common.Keys;
using TFA.Application.Errors;
using TFA.Application.Features.GameweekFinished;
using TFA.Application.Interfaces.Repositories;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Gameweek;
using TFA.Infrastructure.Dtos.Player;
using TFA.Infrastructure.Services.Common;

namespace TFA.Infrastructure.Services;

public sealed class GameweekSummaryService(
    ILogger<GameweekSummaryService> logger,
    IBaseDataService baseData,
    IFotmobService fotmob,
    IGameweekDetailsService gameweekLive,
    IFirebaseRepository db) : AbstractSummaryService<GameweekSummaryData>
{
    public override async Task<ErrorOr<GameweekSummaryData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType, cancellationToken);
        if (fantasyData.IsError)
        {
            logger.LogDataFetchingError(typeof(KeyedBaseData), typeof(GameweekSummaryService), fantasyData.Errors.ToErrorString());
            return fantasyData.ErrorsOrEmptyList;
        }

        Gameweek? latestCheckedDeadlineGameweek = await GetLatestCheckedDeadlineGameweek(fantasyData.Value, fantasyType);
        if (latestCheckedDeadlineGameweek is null)
        {
            logger.LogCouldNotProceedBecauseOfInvalidData(typeof(GameweekSummaryService), typeof(Gameweek));
            return Errors.Service.InvalidData;
        }

        if (!(await ShouldProceedWithGameweekFinishedSummary(fantasyType, latestCheckedDeadlineGameweek)))
        {
            logger.LogJobSkipped(typeof(GameweekSummaryService), "No new finished gameweek to summarize.");
            return Errors.Service.Skipped;
        }

        ILookup<int, FixtureDetails> fixtureDetailsById = (await fotmob.GetFixtureDetailsForGameweek(fantasyData.Value, latestCheckedDeadlineGameweek, fantasyType, cancellationToken))
            .ToLookup(fixture => fixture.FixtureId);

        List<GameweekSummaryPlayer> topPerformingPlayers = [];
        await foreach (FantasyGameweekLivePlayerRequest player in GetTopPerformingPlayers(fantasyType, latestCheckedDeadlineGameweek.Id, 10))
        {
            IReadOnlyList<FixtureDetails> playerFotmobFixtureDetails = player.GameweekDetails?
                .SelectMany(fixture => fixtureDetailsById[fixture.FixtureId])
                .ToList() ?? [];

            IReadOnlyList<FantasyPlayerFixtureHistoryRequest> playerFantasyFixtureDetails =
                await gameweekLive.GetFixturesDetailsForPlayer(
                    fantasyType,
                    player.PlayerId,
                    playerFotmobFixtureDetails.Select(fixture => fixture.FixtureId).ToHashSet(),
                    cancellationToken);

            Player fantasyPlayer = fantasyData.Value.PlayersById[player.PlayerId];
            topPerformingPlayers.Add((
                fantasyPlayer,
                fantasyData.Value.TeamsById[fantasyPlayer.TeamId],
                playerFotmobFixtureDetails,
                playerFantasyFixtureDetails).Adapt<GameweekSummaryPlayer>());
        }

        IReadOnlyList<GameweekSummaryTeam> teamsWithBestUpcomingFixtures = GetTeamsOrderedByFixtureDifficulty(
            fantasyData.Value,
            latestCheckedDeadlineGameweek.Id + 1,
            latestCheckedDeadlineGameweek.Id + 3,
            numberOfTeams: 5,
            MapOpponent,
            MapTeam);

        return new GameweekSummaryData(
            fantasyType, 
            latestCheckedDeadlineGameweek, 
            topPerformingPlayers, 
            teamsWithBestUpcomingFixtures);
    }

    private async ValueTask<bool> ShouldProceedWithGameweekFinishedSummary(FantasyType fantasyType, Gameweek latestCheckedDeadline)
        => Env.IsDevelopment() || (latestCheckedDeadline.IsFinished && latestCheckedDeadline.Id > await GetLatestCheckedFinishedGameweekId(fantasyType));

    private async ValueTask<Gameweek?> GetLatestCheckedDeadlineGameweek(KeyedBaseData fantasyData, FantasyType fantasyType)
    {
        // Keep GW1 in development for easier debugging...
        // Todo: Remove when not WIP
        int lastCheckedDeadline = Env.IsDevelopment()
            ? 1
            : await db.Get<int>(fantasyType.GetDataKey(KeyType.LastCheckedDeadline));

        return fantasyData.GameweeksById.TryGetValue(lastCheckedDeadline, out Gameweek? gameweek)
            ? gameweek
            : null;
    }

    private ValueTask<int> GetLatestCheckedFinishedGameweekId(FantasyType fantasyType)
        => db.Get<int>(fantasyType.GetDataKey(KeyType.LastCheckedFinishedGameweek));

    private async IAsyncEnumerable<FantasyGameweekLivePlayerRequest> GetTopPerformingPlayers(FantasyType fantasyType, int gameweek, int amount)
    {
        foreach (
            FantasyGameweekLivePlayerRequest? player in ((await gameweekLive.GetGameweekDetailsData(fantasyType, gameweek))?
            .Players ?? [])
            .Where(player => player is not null)
            .OrderByDescending(player => player.GameweekStats?.Points ?? 0)
            .Take(amount))
        {
            yield return player;
        }

        yield break;
    }

    private GameweekSummaryTeamOpponent MapOpponent(KeyedBaseData fantasyData, Team team, Fixture fixture, bool isDouble)
    {
        bool isHome = fixture.HomeTeamId == team.Id;
        return new GameweekSummaryTeamOpponent(
            fixture.Id,
            fixture.GameweekId ?? 1,
            GetOpponentShortName(fantasyData, isHome, fixture.HomeTeamId, fixture.AwayTeamId),
            GetFixtureDifficulty(isHome, fixture),
            isHome);
    }

    private GameweekSummaryTeam MapTeam(Team team, IReadOnlyList<GameweekSummaryTeamOpponent> opponents, int blankGameweeks)
        => new(
            team.Id,
            team.Name,
            team.ShortName,
            team.Position ?? 99,
            opponents.Sum(opp => opp.FixtureDifficulty) + Math.Max((blankGameweeks * 6), 0),
            opponents.OrderBy(opp => opp.Gameweek).ToList());
}
