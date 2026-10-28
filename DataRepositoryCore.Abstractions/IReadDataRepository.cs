namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Defines read operations for entities. The default implementation uses untracked queries.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
public interface IReadDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>Determines whether any entities match the query.</summary>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>Determines whether any entities match the query.</summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the query as an asynchronously enumerable sequence.</summary>
    IAsyncEnumerable<TEntity> AsAsyncEnumerable();

    /// <summary>Returns a composable query for entities.</summary>
    IQueryable<TEntity> AsQueryable();

    /// <summary>Calculates the average of the selected integer values.</summary>
    Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);

    /// <summary>Determines whether the query contains the specified entity.</summary>
    Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Counts entities matching the query.</summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Counts entities matching the query.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the first matching entity, or throws if none exists.</summary>
    Task<TEntity> FirstAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the first matching entity, or throws if none exists.</summary>
    Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the first matching entity, or null if none exists.</summary>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the first matching entity, or null if none exists.</summary>
    Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Invokes an action for each entity in the query.</summary>
    Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default);

    /// <summary>Returns a query that disables configured automatic includes.</summary>
    IQueryable<TEntity> IgnoreAutoIncludes();

    /// <summary>Returns a query that ignores configured query filters.</summary>
    IQueryable<TEntity> IgnoreQueryFilters();

    /// <summary>Explicitly loads a collection navigation for the specified entity.</summary>
    /// <remarks>The default EF Core implementation can load navigations for a detached entity without attaching it. The repository context must still be usable.</remarks>
    Task LoadAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class;

    /// <summary>Explicitly loads a reference navigation for the specified entity.</summary>
    /// <remarks>The default EF Core implementation can load navigations for a detached entity without attaching it. The repository context must still be usable.</remarks>
    Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class;

    /// <summary>Returns the maximum entity or selected value in the query.</summary>
    Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);

    /// <summary>Returns the maximum entity or selected value in the query.</summary>
    Task<TEntity> MaxAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the minimum entity or selected value in the query.</summary>
    Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);

    /// <summary>Returns the minimum entity or selected value in the query.</summary>
    Task<TEntity> MinAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, or throws if there is not exactly one.</summary>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, or throws if there is not exactly one.</summary>
    Task<TEntity> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, null if none exists, or throws if multiple match.</summary>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, null if none exists, or throws if multiple match.</summary>
    Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Calculates the sum of the selected integer values.</summary>
    Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default);

    /// <summary>Executes the query and returns its entities in an array.</summary>
    Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query and returns its entities in a list.</summary>
    Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a diagnostic representation of the query.</summary>
    string ToQueryString();

    /// <summary>Returns a query filtered by the specified predicate.</summary>
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Executes a query filtered by the specified predicate and returns a list.</summary>
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
