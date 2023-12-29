using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Services.BaseData;
using TFA.Domain.Models;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;

namespace TFA.Application.Features.BaseData.Queries;

public sealed class BaseDataQueryHandler(
    IFirebaseRepository db) : IRequestHandler<BaseDataQuery<IEntity>, IEnumerable<IEntity>>
{
    public async Task<IEnumerable<IEntity>> Handle(BaseDataQuery<IEntity> request, CancellationToken cancellationToken)
    {
        string dataKey = request.Filter.FantasyType.GetDataKey(KeyType.BaseData);
        FantasyBaseData? data = await db.Get<FantasyBaseData>(dataKey);

        IEnumerable<IEntity> entitiesToFilter =
            request.Filter.EntityType.Name switch
            {
                nameof(Player) => data.Players,
                nameof(Team) => data.Teams,
                nameof(Gameweek) => data.Gameweeks,
                nameof(Fixture) => data.Fixtures,
                _ => throw new NotImplementedException()
            };

        return request.Filter.Apply(entitiesToFilter);
    }
}
