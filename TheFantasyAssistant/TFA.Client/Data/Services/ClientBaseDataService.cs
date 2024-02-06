using ErrorOr;
using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;
using TFA.Domain.Models;

namespace TFA.Client.Data.Services;

public class ClientBaseDataService(IReadOnlyFirebaseRepository db) : IBaseDataService
{
    public async Task<ErrorOr<FantasyBaseData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        string dataKey = fantasyType.GetDataKey(KeyType.BaseData);
        return await db.Get<FantasyBaseData>(dataKey, cancellationToken);
    }

    public Task<ErrorOr<KeyedBaseData>> GetKeyedData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
