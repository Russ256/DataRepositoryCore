namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>A composable query supporting scalar, DTO, and entity results without EF Core types.</summary>
/// <typeparam name="T">The result type.</typeparam>
/// <remarks>Composition creates a new query. Execute while the context is usable. Default-returning methods return default(T) when empty.</remarks>
public interface IRepositoryResultQuery<T>
{
    /// <summary>Executes the query using AnyAsync.</summary>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using AnyAsync with a predicate.</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using CountAsync.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using CountAsync with a predicate.</summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using LongCountAsync.</summary>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using LongCountAsync with a predicate.</summary>
    Task<long> LongCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using FirstAsync.</summary>
    Task<T> FirstAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using FirstAsync with a predicate.</summary>
    Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using FirstOrDefaultAsync.</summary>
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using FirstOrDefaultAsync with a predicate.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using SingleAsync.</summary>
    Task<T> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using SingleAsync with a predicate.</summary>
    Task<T> SingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query using SingleOrDefaultAsync.</summary>
    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query using SingleOrDefaultAsync with a predicate.</summary>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Determines whether every result satisfies the predicate.</summary>
    Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Determines whether the query contains an item.</summary>
    Task<bool> ContainsAsync(T item, CancellationToken cancellationToken = default);

    /// <summary>Invokes an action for each result.</summary>
    Task ForEachAsync(Action<T> action, CancellationToken cancellationToken = default);

    /// <summary>Returns the minimum result.</summary>
    Task<T> MinAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the minimum selected value.</summary>
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

    /// <summary>Returns the maximum result.</summary>
    Task<T> MaxAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the maximum selected value.</summary>
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<int> SumAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<long> SumAsync(Expression<Func<T, long>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double> AverageAsync(Expression<Func<T, long>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<float> SumAsync(Expression<Func<T, float>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<float> AverageAsync(Expression<Func<T, float>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<double> SumAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double> AverageAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<int?> SumAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double?> AverageAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<long?> SumAsync(Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double?> AverageAsync(Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<float?> SumAsync(Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<float?> AverageAsync(Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<double?> SumAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<double?> AverageAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of selected values.</summary>
    Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Calculates the average of selected values.</summary>
    Task<decimal?> AverageAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default);

    /// <summary>Returns the results as a list.</summary>
    Task<List<T>> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the results as an array.</summary>
    Task<T[]> ToArrayAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns an asynchronous sequence; enumeration accepts a cancellation token.</summary>
    IAsyncEnumerable<T> AsAsyncEnumerable();

    /// <summary>Returns a diagnostic representation of the query.</summary>
    string ToQueryString();

    /// <summary>Filters the results.</summary>
    IRepositoryResultQuery<T> Where(Expression<Func<T, bool>> predicate);

    /// <summary>Skips the specified number of results; predictable paging requires a query with ordering.</summary>
    IRepositoryResultQuery<T> Skip(int count);

    /// <summary>Takes the specified number of results; predictable paging requires a query with ordering.</summary>
    IRepositoryResultQuery<T> Take(int count);

    /// <summary>Returns distinct results.</summary>
    IRepositoryResultQuery<T> Distinct();

    /// <summary>Projects results into scalars or objects.</summary>
    IRepositoryResultQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
}
