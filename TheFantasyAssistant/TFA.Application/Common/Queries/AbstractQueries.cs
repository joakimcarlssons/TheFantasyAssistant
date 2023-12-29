using TFA.Domain.Models;

namespace TFA.Application.Common.Queries;

public abstract class AbstractDataQuery<TEntity>(IQueryFilter<TEntity> filter) : IRequest<IEnumerable<TEntity>>
    where TEntity : IEntity
{
    public IQueryFilter<TEntity> Filter { get; init; } = filter;
    public List<IEntity>? Data { get; set; } = [];
}
