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
    protected readonly IDataContext dataContext;
    protected readonly DbSet<TEntity> entities;
    protected readonly ILogger<ReadDataRepository<TEntity, TKey>> logger;

    public ReadDataRepository(IDataContext dataContext, ILogger<ReadDataRepository<TEntity, TKey>> logger = null)
    {
        this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        this.entities = this.dataContext.GetDbSet<TEntity>();
        this.logger = logger;
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().Where(predicate).AnyAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().AnyAsync(cancellationToken);
    }

    public IAsyncEnumerable<TEntity> AsAsyncEnumerable()
    {
        return this.AsQueryable().AsAsyncEnumerable();
    }

    public virtual IQueryable<TEntity> AsQueryable()
    {
        this.logger?.LogTrace($"Returning untracked IQuerable");
        return this.entities.AsNoTracking();
    }

    public Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().AverageAsync(selector, cancellationToken);
    }

    public Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().ContainsAsync(entity, cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().CountAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().CountAsync(cancellationToken);
    }

    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().FirstAsync(predicate, cancellationToken);
    }

    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().FirstAsync(cancellationToken);
    }

    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().FirstOrDefaultAsync(cancellationToken);
    }

    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().ForEachAsync(action, cancellationToken);
    }

    public IQueryable<TEntity> IgnoreAutoIncludes()
    {
        return this.AsQueryable().IgnoreAutoIncludes();
    }

    public IQueryable<TEntity> IgnoreQueryFilters()
    {
        return this.AsQueryable().IgnoreQueryFilters();
    }

    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().LastAsync(predicate, cancellationToken);
    }

    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().LastAsync(cancellationToken);
    }

    public Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().LastOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> LastOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().LastOrDefaultAsync(cancellationToken);
    }

    public Task LoadAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken = default)
                                                                where TProperty : class
    {
        return this.dataContext.Entry(entity).Collection(propertyExpression).LoadAsync(cancellationToken);
    }

    public Task LoadReferenceAsync<TProperty>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken = default) where TProperty : class
    {
        return this.dataContext.Entry(entity).Reference(propertyExpression).LoadAsync(cancellationToken);
    }

    public Task<TEntity> MaxAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().MaxAsync(cancellationToken);
    }

    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().MaxAsync(selector, cancellationToken);
    }

    public Task<TEntity> MinAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().MinAsync(cancellationToken);
    }

    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().MinAsync(selector, cancellationToken);
    }

    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().SingleAsync(predicate, cancellationToken);
    }

    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().SingleAsync(cancellationToken);
    }

    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().SingleOrDefaultAsync(cancellationToken);
    }

    public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().SumAsync(selector, cancellationToken);
    }

    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().ToArrayAsync(cancellationToken);
    }

    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return this.AsQueryable().ToListAsync(cancellationToken);
    }

    public string ToQueryString()
    {
        return this.AsQueryable().ToQueryString();
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return this.AsQueryable().Where(predicate);
    }

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return this.Where(predicate).ToListAsync(cancellationToken);
    }
}