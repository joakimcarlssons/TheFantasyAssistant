using TFA.Application.Config;
using TFA.Application.Errors;
using TFA.Application.Features.FixtureLiveUpdate;
using TFA.Domain.Exceptions;
using TFA.Infrastructure.Dtos.Gameweek;
using TFA.Infrastructure.Dtos.Player;

namespace TFA.Infrastructure.Services;

public interface IGameweekDetailsService : IDataService<ErrorOr<GameweekLiveUpdateData>>
{
    Task<FantasyGameweekLiveRequest?> GetGameweekDetailsData(FantasyType fantasyType, int gameweek, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FantasyPlayerFixtureHistoryRequest>> GetFixturesDetailsForPlayer(FantasyType fantasyType, int playerId, IReadOnlySet<int> fixtureIds, CancellationToken cancellationToken);
}

public sealed class GameweekDetailsService(
    HttpClient httpClient,
    IBaseDataService fantasyData,
    ILogger<GameweekDetailsService> logger,
    IOptions<SourceOptions> sourceOptions) : IGameweekDetailsService
{
    private readonly SourceOptions sources = sourceOptions.Value;

    public async Task<ErrorOr<GameweekLiveUpdateData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        ErrorOr<FantasyBaseData> baseData = await fantasyData.GetData(fantasyType, cancellationToken);
        if (baseData.IsError)
        {
            return baseData.Errors;
        }

        if (baseData.Value.Gameweeks.FirstOrDefault(gw => gw.IsCurrent) is { Id: > 0 } currentGameweek)
        {
            if (await GetGameweekDetailsData(fantasyType, currentGameweek.Id, cancellationToken) is { } gameweekLiveData)
            {
                // Map GameweekLiveData to GameweekLiveUpdateData and return
            }

            logger.LogDataFetchingError(typeof(FantasyGameweekLiveRequest), typeof(GameweekDetailsService), $"Could not find gameweek data for current gameweek {currentGameweek.Id}");
            return Errors.Service.Fetching;
        }

        logger.LogJobSkipped(typeof(GameweekDetailsService), $"No current gameweek exists for fantasy type {fantasyType}.");
        return Errors.Service.Skipped;
    }

    public async Task<IReadOnlyList<FantasyPlayerFixtureHistoryRequest>> GetFixturesDetailsForPlayer(FantasyType fantasyType, int playerId, IReadOnlySet<int> fixtureIds, CancellationToken cancellationToken)
    {
        string url = fantasyType switch
        {
            FantasyType.FPL => sources.FPL.PlayerHistory,
            FantasyType.Allsvenskan => sources.FAS.PlayerHistory,
            _ => throw new FantasyTypeNotSupportedException()
        };

        FantasyPlayerHistoryRequest? playerHistory = await httpClient.GetFromJsonAsync<FantasyPlayerHistoryRequest>(string.Concat(url, $"/{playerId}"), cancellationToken);
        return playerHistory is { FixtureHistory.Count: > 0 }
            ? playerHistory.FixtureHistory
                .Where(fixture => fixtureIds.Contains(fixture.FixtureId))
                .ToList()
            : [];
    }

    public Task<FantasyGameweekLiveRequest?> GetGameweekDetailsData(FantasyType fantasyType, int gameweek, CancellationToken cancellationToken = default)
    {
        string url = fantasyType switch
        {
            FantasyType.FPL => sources.FPL.GameweekLive,
            FantasyType.Allsvenskan => sources.FAS.GameweekLive,
            _ => throw new FantasyTypeNotSupportedException()
        };

        return httpClient.GetFromJsonAsync<FantasyGameweekLiveRequest>(string.Concat(url, gameweek, "/live"), cancellationToken);
    }
}
