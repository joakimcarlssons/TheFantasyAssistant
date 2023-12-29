using TFA.Application.Common.Queries;
using TFA.Domain.Models;

namespace TFA.Application.Features.BaseData.Queries;

public class BaseDataQuery<TEntity>(IQueryFilter<TEntity> filter) : AbstractDataQuery<TEntity>(filter)
    where TEntity : IEntity
{
}

public sealed record BaseDataQueryFilter<TEntity>(
    FantasyType FantasyType, 
    Type EntityType,
    Func<IEnumerable<TEntity>, IEnumerable<TEntity>> Filter) : IQueryFilter<TEntity>
    where TEntity : IEntity;

public sealed record BaseDataQueryResult<TData>(
    IReadOnlyList<TData> Data) where TData : IEntity;