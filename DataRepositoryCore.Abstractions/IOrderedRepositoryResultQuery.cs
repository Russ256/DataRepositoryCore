namespace DataRepositoryCore;

using System;
using System.Linq.Expressions;

/// <summary>An ordered result query supporting additional sort keys.</summary>
/// <typeparam name="T">The result type.</typeparam>
public interface IOrderedRepositoryResultQuery<T> : IRepositoryResultQuery<T>
{
    /// <summary>Adds an ascending sort key after the existing keys.</summary>
    IOrderedRepositoryResultQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);

    /// <summary>Adds a descending sort key after the existing keys.</summary>
    IOrderedRepositoryResultQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
}
