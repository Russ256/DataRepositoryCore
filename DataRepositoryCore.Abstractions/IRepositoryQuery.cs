namespace DataRepositoryCore;

using System;
using System.Linq.Expressions;

/// <summary>An entity query supporting eager loading and asynchronous execution.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IRepositoryQuery<TEntity> : IRepositoryResultQuery<TEntity> where TEntity : class
{
    /// <summary>Orders entities by an ascending key, replacing any previous ordering.</summary>
    new IOrderedRepositoryQuery<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>Orders entities by a descending key, replacing any previous ordering.</summary>
    new IOrderedRepositoryQuery<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>Includes a reference or collection navigation.</summary>
    IIncludableRepositoryQuery<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation);

    /// <summary>Filters entities, preserving eager-loading support.</summary>
    new IRepositoryQuery<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Skips results. Predictable paging requires a query with ordering.</summary>
    new IRepositoryQuery<TEntity> Skip(int count);

    /// <summary>Takes results. Predictable paging requires a query with ordering.</summary>
    new IRepositoryQuery<TEntity> Take(int count);

    /// <summary>Returns distinct entities.</summary>
    new IRepositoryQuery<TEntity> Distinct();
}
