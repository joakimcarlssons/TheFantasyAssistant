using ErrorOr;
using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;
using TFA.Domain.Models;

namespace TFA.Client.Shared.Data.Services;

public class ClientBaseDataService(IReadOnlyFirebaseRepository db) : IBaseDataService
{
    public async Task<ErrorOr<FantasyBaseData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        string dataKey = fantasyType.GetDataKey(KeyType.BaseData);
        return await db.Get<FantasyBaseData>(dataKey, cancellationToken);
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
}
