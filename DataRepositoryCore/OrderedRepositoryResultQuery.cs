namespace DataRepositoryCore;

using System;
using System.Linq;
using System.Linq.Expressions;

internal sealed class OrderedRepositoryResultQuery<T>(IOrderedQueryable<T> query)
    : RepositoryResultQuery<T>(query), IOrderedRepositoryResultQuery<T>
{
    public IOrderedRepositoryResultQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new OrderedRepositoryResultQuery<T>(query.ThenBy(keySelector));
    }

    public IOrderedRepositoryResultQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        return new OrderedRepositoryResultQuery<T>(query.ThenByDescending(keySelector));
    }
}
