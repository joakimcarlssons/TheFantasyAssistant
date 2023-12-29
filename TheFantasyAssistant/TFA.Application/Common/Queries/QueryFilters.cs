using TFA.Domain.Models;

namespace TFA.Application.Common.Queries;

public interface IQueryFilter<TEntity>
    where TEntity : IEntity
{
    FantasyType FantasyType { get; }
    Type EntityType { get; }
    Func<IEnumerable<TEntity>, IEnumerable<TEntity>> Filter { get; }

    IEnumerable<TEntity> Apply(IEnumerable<TEntity> items) => Filter.Invoke(items);
}

public static class QueryFilterExtensions
{
    public static IEnumerable<IEntity> ApplyFilters<TEntity>(
        this IEnumerable<IEntity> entities,
        Func<IEnumerable<TEntity>, IEnumerable<TEntity>> filter)
        where TEntity : IEntity
    {
        return (IEnumerable<IEntity>)filter(entities.Cast<TEntity>());
    }
}