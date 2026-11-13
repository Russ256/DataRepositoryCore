namespace DataRepositoryCore;

using System;
using System.Linq.Expressions;

/// <summary>An ordered entity query supporting additional sort keys and eager loading.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IOrderedRepositoryQuery<TEntity> : IRepositoryQuery<TEntity>, IOrderedRepositoryResultQuery<TEntity>
    where TEntity : class
{
    /// <summary>Adds an ascending sort key after the existing keys.</summary>
    new IOrderedRepositoryQuery<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

    /// <summary>Adds a descending sort key after the existing keys.</summary>
    new IOrderedRepositoryQuery<TEntity> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);
}
