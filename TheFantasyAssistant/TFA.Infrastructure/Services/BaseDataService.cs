using TFA.Application.Config;
using TFA.Application.Http;
using TFA.Application.Services.BaseData;
using TFA.Domain.Exceptions;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fixture;
using TFA.Infrastructure.Models;

namespace TFA.Infrastructure.Services;

public interface IBaseDataService : IDataService<ErrorOr<FantasyBaseData>>
{
    Task<ErrorOr<KeyedBaseData>> GetKeyedData(FantasyType fantasyType, CancellationToken cancellationToken = default);
}

public class BaseDataService(
    HttpClient httpClient, 
    IOptions<SourceOptions> sourceOptions, 
    IMapper mapper) : IBaseDataService
{
    private readonly SourceOptions sources = sourceOptions.Value;

    public async Task<ErrorOr<FantasyBaseData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        ErrorOr<FantasyBaseDataRequest> baseData = await GetBaseData(fantasyType, cancellationToken);
        if (baseData.IsError)
        {
            return baseData.ErrorsOrEmptyList;
        }

        ErrorOr<IReadOnlyList<FantasyFixtureRequest>> fixtureData = await GetFixtureData(fantasyType, cancellationToken);
        if (fixtureData.IsError)
        {
            return fixtureData.ErrorsOrEmptyList;
        }

        // Map results
        IReadOnlyList<Player> players = mapper.Map<IReadOnlyList<Player>>(baseData.Value.Players);
        IReadOnlyList<Team> teams = mapper.Map<IReadOnlyList<Team>>(baseData.Value.Teams);
        IReadOnlyList<Gameweek> gameweeks = mapper.Map<IReadOnlyList<Gameweek>>(baseData.Value.Gameweeks);
        IReadOnlyList<Fixture> fixtures = mapper.Map<IReadOnlyList<Fixture>>(fixtureData.Value);

        return new FantasyBaseData(players, teams, gameweeks, fixtures);
    }

    public async Task<ErrorOr<KeyedBaseData>> GetKeyedData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        ErrorOr<FantasyBaseData> dataWrapper = await GetData(fantasyType, cancellationToken);

        if (!dataWrapper.IsError)
        {
            FantasyBaseData data = dataWrapper.Value;
            return new KeyedBaseData(
                data.Players.ToDictionary(p => p.Id),
                data.Players.ToLookup(p => p.TeamId),
                data.Teams.ToDictionary(t => t.Id),
                data.Teams.ToDictionary(t => t.Name),
                data.Gameweeks.ToDictionary(gw => gw.Id),
                data.Fixtures.ToDictionary(f => f.Id),
                data.Fixtures.ToLookup(f => f.GameweekId ?? -1),
                data.Fixtures.ToLookup(f => f.HomeTeamId),
                data.Fixtures.ToLookup(f => f.AwayTeamId));
        }

        return dataWrapper.Errors;
    }

    private FantasyBaseDataOptions GetFantasyBaseDataOptions(FantasyType fantasyType)
        => fantasyType switch
        {
            FantasyType.FPL => sources.FPL,
            FantasyType.Allsvenskan => sources.FAS,
            _ => throw new FantasyTypeNotSupportedException()
        };

    private async Task<ErrorOr<FantasyBaseDataRequest>> GetBaseData(FantasyType fantasyType, CancellationToken cancellationToken)
    {
        FantasyBaseDataOptions options = GetFantasyBaseDataOptions(fantasyType);
        return await httpClient.TryGetAsJsonAsync<FantasyBaseDataRequest>(options.Base, cancellationToken);
    }

    private async Task<ErrorOr<IReadOnlyList<FantasyFixtureRequest>>> GetFixtureData(FantasyType fantasyType, CancellationToken cancellationToken)
    {
        FantasyBaseDataOptions options = GetFantasyBaseDataOptions(fantasyType);
        return await httpClient.TryGetAsJsonAsync<IReadOnlyList<FantasyFixtureRequest>>(options.Fixtures, cancellationToken);
    }
}

public record KeyedBaseData(
    IReadOnlyDictionary<int, Player> PlayersById,
    ILookup<int, Player> PlayersByTeamId,
    IReadOnlyDictionary<int, Team> TeamsById,
    IReadOnlyDictionary<string, Team> TeamsByName,
    IReadOnlyDictionary<int, Gameweek> GameweeksById,
    IReadOnlyDictionary<int, Fixture> FixturesById,
    ILookup<int, Fixture> FixturesByGameweekId,
    ILookup<int, Fixture> FixturesByHomeTeamId,
    ILookup<int, Fixture> FixturesByAwayTeamId);