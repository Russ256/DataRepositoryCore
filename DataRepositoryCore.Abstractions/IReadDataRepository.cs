namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public interface IReadDataRepository<TEntity, TKey>
    where TEntity : IEntity<TKey>
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TEntity> AsAsyncEnumerable();

    IQueryable<TEntity> AsQueryable();

    Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);

    Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<TEntity> FirstAsync(CancellationToken cancellationToken = default);

    Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default);

    IQueryable<TEntity> IgnoreAutoIncludes();

    IQueryable<TEntity> IgnoreQueryFilters();

    Task<TEntity> LastAsync(CancellationToken cancellationToken = default);

    Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> LastOrDefaultAsync(CancellationToken cancellationToken = default);

    Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task LoadAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class;

    Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class;

    Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);

    Task<TEntity> MaxAsync(CancellationToken cancellationToken = default);

    Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);

    Task<TEntity> MinAsync(CancellationToken cancellationToken = default);

    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> SingleAsync(CancellationToken cancellationToken = default);

    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);

    Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default);

    Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

    string ToQueryString();

    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}