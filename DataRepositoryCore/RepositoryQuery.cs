namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

internal class RepositoryQuery<TEntity>(IQueryable<TEntity> query) : IRepositoryQuery<TEntity>
    where TEntity : class
{
    public IIncludableRepositoryQuery<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation)
    {
        return new IncludableRepositoryQuery<TEntity, TProperty>(query.Include(navigation));
    }

    public IRepositoryQuery<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return new RepositoryQuery<TEntity>(query.Where(predicate));
    }

    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
    {
        return query.SingleAsync(cancellationToken);
    }

    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return query.SingleAsync(predicate, cancellationToken);
    }

    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return query.ToListAsync(cancellationToken);
    }

    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default)
    {
        return query.ToArrayAsync(cancellationToken);
    }
}
