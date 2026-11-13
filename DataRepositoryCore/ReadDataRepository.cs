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
/// Repository for read access. Its queries do not track entities by default.
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository returns</typeparam>
/// <typeparam name="TKey">The type of the entity key</typeparam>
public class ReadDataRepository<TEntity, TKey> : IReadDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>The context used by this repository.</summary>
    protected readonly IDataContext DataContext;

    /// <summary>The entity set queried by this repository.</summary>
    protected readonly DbSet<TEntity> Entities;

    /// <summary>The optional logger used by this repository.</summary>
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
        return Query().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Query().AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable()
    {
        return Query().AsAsyncEnumerable();
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
    public IRepositoryQuery<TEntity> Query()
    {
        return new RepositoryQuery<TEntity>(AsQueryable());
    }

    /// <inheritdoc />
    public Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return Query().AverageAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Query().ContainsAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Query().CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().FirstAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
    {
        return Query().FirstAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        return Query().ForEachAsync(action, cancellationToken);
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
    public IIncludableRepositoryQuery<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation)
    {
        return Query().Include(navigation);
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
        return Query().MaxAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return Query().MaxAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> MinAsync(CancellationToken cancellationToken = default)
    {
        return Query().MinAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return Query().MinAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().SingleAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return Query().SumAsync(selector, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return Query().ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return Query().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public string ToQueryString()
    {
        return Query().ToQueryString();
    }

    /// <inheritdoc />
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().Where(predicate);
    }

    /// <inheritdoc />
    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Query().Where(predicate).ToListAsync(cancellationToken);
    }
}
