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
    protected readonly IDataContext DataContext;
    protected readonly DbSet<TEntity> Entities;
    protected readonly ILogger<ReadDataRepository<TEntity, TKey>> Logger;

    public ReadDataRepository(IDataContext dataContext, ILogger<ReadDataRepository<TEntity, TKey>> logger = null)
    {
        DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        Entities = DataContext.GetDbSet<TEntity>();
        Logger = logger;
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().Where(predicate).AnyAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().AnyAsync(cancellationToken);
    }

    public IAsyncEnumerable<TEntity> AsAsyncEnumerable()
    {
        return AsQueryable().AsAsyncEnumerable();
    }

    public virtual IQueryable<TEntity> AsQueryable()
    {
        Logger?.LogTrace("Returning untracked IQueryable");
        return Entities.AsNoTracking();
    }

    public Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().AverageAsync(selector, cancellationToken);
    }

    public Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return AsQueryable().ContainsAsync(entity, cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().CountAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().CountAsync(cancellationToken);
    }

    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstAsync(predicate, cancellationToken);
    }

    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstAsync(cancellationToken);
    }

    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().FirstOrDefaultAsync(cancellationToken);
    }

    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        return AsQueryable().ForEachAsync(action, cancellationToken);
    }

    public IQueryable<TEntity> IgnoreAutoIncludes()
    {
        return AsQueryable().IgnoreAutoIncludes();
    }

    public IQueryable<TEntity> IgnoreQueryFilters()
    {
        return AsQueryable().IgnoreQueryFilters();
    }

    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().LastAsync(predicate, cancellationToken);
    }

    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().LastAsync(cancellationToken);
    }

    public Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().LastOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> LastOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().LastOrDefaultAsync(cancellationToken);
    }

    public Task LoadAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken = default)
                                                                where TProperty : class
    {
        return DataContext.Entry(entity).Collection(propertyExpression).LoadAsync(cancellationToken);
    }

    public Task LoadReferenceAsync<TProperty>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class
    {
        return DataContext.Entry(entity).Reference(propertyExpression).LoadAsync(cancellationToken);
    }

    public Task<TEntity> MaxAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().MaxAsync(cancellationToken);
    }

    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().MaxAsync(selector, cancellationToken);
    }

    public Task<TEntity> MinAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().MinAsync(cancellationToken);
    }

    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().MinAsync(selector, cancellationToken);
    }

    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleAsync(predicate, cancellationToken);
    }

    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleAsync(cancellationToken);
    }

    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().SingleOrDefaultAsync(cancellationToken);
    }

    public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return AsQueryable().SumAsync(selector, cancellationToken);
    }

    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().ToArrayAsync(cancellationToken);
    }

    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().ToListAsync(cancellationToken);
    }

    public string ToQueryString()
    {
        return AsQueryable().ToQueryString();
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().Where(predicate);
    }

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Where(predicate).ToListAsync(cancellationToken);
    }
}