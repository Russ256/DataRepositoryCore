namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

internal class RepositoryResultQuery<T>(IQueryable<T> query) : IRepositoryResultQuery<T>
{
    protected IQueryable<T> Query { get; } = query;

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Query.AnyAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.AnyAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Query.CountAsync(cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.CountAsync(predicate, cancellationToken);
    }

    public Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        return Query.LongCountAsync(cancellationToken);
    }

    public Task<long> LongCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.LongCountAsync(predicate, cancellationToken);
    }

    public Task<T> FirstAsync(CancellationToken cancellationToken = default)
    {
        return Query.FirstAsync(cancellationToken);
    }

    public Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.FirstAsync(predicate, cancellationToken);
    }

    public Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<T> SingleAsync(CancellationToken cancellationToken = default)
    {
        return Query.SingleAsync(cancellationToken);
    }

    public Task<T> SingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.SingleAsync(predicate, cancellationToken);
    }

    public Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query.AllAsync(predicate, cancellationToken);
    }

    public Task<bool> ContainsAsync(T item, CancellationToken cancellationToken = default)
    {
        return Query.ContainsAsync(item, cancellationToken);
    }

    public Task ForEachAsync(Action<T> action, CancellationToken cancellationToken = default)
    {
        return Query.ForEachAsync(action, cancellationToken);
    }

    public Task<T> MinAsync(CancellationToken cancellationToken = default)
    {
        return Query.MinAsync(cancellationToken);
    }

    public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return Query.MinAsync(selector, cancellationToken);
    }

    public Task<T> MaxAsync(CancellationToken cancellationToken = default)
    {
        return Query.MaxAsync(cancellationToken);
    }

    public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return Query.MaxAsync(selector, cancellationToken);
    }

    public Task<int> SumAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<long> SumAsync(Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double> AverageAsync(Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<float> SumAsync(Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<float> AverageAsync(Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<double> SumAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double> AverageAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<int?> SumAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double?> AverageAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<long?> SumAsync(Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double?> AverageAsync(Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<float?> SumAsync(Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<float?> AverageAsync(Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<double?> SumAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<double?> AverageAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.SumAsync(selector, cancellationToken);
    }

    public Task<decimal?> AverageAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
    {
        return Query.AverageAsync(selector, cancellationToken);
    }

    public Task<List<T>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return Query.ToListAsync(cancellationToken);
    }

    public Task<T[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return Query.ToArrayAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> AsAsyncEnumerable()
    {
        return Query.AsAsyncEnumerable();
    }

    public string ToQueryString()
    {
        return Query.ToQueryString();
    }

    public IRepositoryResultQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        return new RepositoryResultQuery<T>(Query.Where(predicate));
    }

    public IRepositoryResultQuery<T> Skip(int count)
    {
        return new RepositoryResultQuery<T>(Query.Skip(count));
    }

    public IRepositoryResultQuery<T> Take(int count)
    {
        return new RepositoryResultQuery<T>(Query.Take(count));
    }

    public IRepositoryResultQuery<T> Distinct()
    {
        return new RepositoryResultQuery<T>(Query.Distinct());
    }

    public IRepositoryResultQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
    {
        return new RepositoryResultQuery<TResult>(Query.Select(selector));
    }
}
