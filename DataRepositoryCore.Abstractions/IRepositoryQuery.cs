namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>A composable query with eager loading and asynchronous execution independent of EF Core types.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <remarks>Composition returns a new query. Execute it while the repository context is still usable.</remarks>
public interface IRepositoryQuery<TEntity> where TEntity : class
{
    /// <summary>Includes a reference or collection navigation in the results.</summary>
    IRepositoryQuery<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation);

    /// <summary>Returns a query filtered by the specified predicate.</summary>
    IRepositoryQuery<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>Returns the only entity, or throws if there is not exactly one.</summary>
    Task<TEntity> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, or throws if there is not exactly one.</summary>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns the only entity, null if none exists, or throws if multiple match.</summary>
    Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the only matching entity, null if none exists, or throws if multiple match.</summary>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Executes the query and returns its entities in a list.</summary>
    Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>Executes the query and returns its entities in an array.</summary>
    Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default);
}
