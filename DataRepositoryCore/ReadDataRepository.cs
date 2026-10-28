namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Repository for read on data access.  Entities are not tracked.
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository returns</typeparam>
/// <typeparam name="TKey">The type of the entity key</typeparam>
public class ReadDataRepository<TEntity, TKey> : IReadDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>Stores the repository context or query dependencies.</summary>
    protected readonly IDataContext DataContext;
    /// <summary>Stores the repository context or query dependencies.</summary>
    protected readonly DbSet<TEntity> Entities;
    /// <summary>Stores the repository context or query dependencies.</summary>
    protected readonly ILogger<ReadDataRepository<TEntity, TKey>>? Logger;

    /// <summary>Creates a repository using the supplied context and optional logger.</summary>
    public ReadDataRepository(IDataContext dataContext, ILogger<ReadDataRepository<TEntity, TKey>>? logger = null)
    {
        DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        Entities = DataContext.GetDbSet<TEntity>();
        Logger = logger;
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().Where(predicate).AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable()
    {
        return AsQueryable().AsAsyncEnumerable();
    }

    /// <inheritdoc />
    public virtual IQueryable<TEntity> AsQueryable()
    {
        if (Logger is not null && Logger.IsEnabled(LogLevel.Trace))
        {
            Logger.LogTrace("Returning untracked IQueryable");
        }

        return Entities.AsNoTracking();
    }

    /// <inheritdoc />
    public Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().AverageAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return AsQueryable().ContainsAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        return AsQueryable().ForEachAsync(action, cancellationToken);
    }

    /// <inheritdoc />
    public IQueryable<TEntity> IgnoreAutoIncludes()
    {
        return AsQueryable().IgnoreAutoIncludes();
    }

    /// <inheritdoc />
    public IQueryable<TEntity> IgnoreQueryFilters()
    {
        return AsQueryable().IgnoreQueryFilters();
    }

    /// <inheritdoc />
    public Task LoadAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken = default)
                                                                where TProperty : class
    {
        return DataContext.Entry(entity).Collection(propertyExpression).LoadAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task LoadReferenceAsync<TProperty>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, TProperty?>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class
    {
        return DataContext.Entry(entity).Reference(propertyExpression).LoadAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> MaxAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().MaxAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().MaxAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> MinAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().MinAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().MinAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SumAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public string ToQueryString()
    {
        return AsQueryable().ToQueryString();
    }

    /// <inheritdoc />
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().Where(predicate);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Where(predicate).ToListAsync(cancellationToken);
    }
}
