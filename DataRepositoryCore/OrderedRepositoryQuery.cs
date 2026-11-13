namespace DataRepositoryCore;

using System;
using System.Linq;
using System.Linq.Expressions;

internal sealed class OrderedRepositoryQuery<TEntity>(IOrderedQueryable<TEntity> query)
    : RepositoryQuery<TEntity>(query), IOrderedRepositoryQuery<TEntity> where TEntity : class
{
    public IOrderedRepositoryQuery<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        return new OrderedRepositoryQuery<TEntity>(query.ThenBy(keySelector));
    }

    public IOrderedRepositoryQuery<TEntity> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
    {
        return new OrderedRepositoryQuery<TEntity>(query.ThenByDescending(keySelector));
    }

    IOrderedRepositoryResultQuery<TEntity> IOrderedRepositoryResultQuery<TEntity>.ThenBy<TKey>(
        Expression<Func<TEntity, TKey>> keySelector) => ThenBy(keySelector);

    IOrderedRepositoryResultQuery<TEntity> IOrderedRepositoryResultQuery<TEntity>.ThenByDescending<TKey>(
        Expression<Func<TEntity, TKey>> keySelector) => ThenByDescending(keySelector);
}
