namespace DataRepositoryCore;

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

internal class RepositoryQuery<TEntity>(IQueryable<TEntity> query)
    : RepositoryResultQuery<TEntity>(query), IRepositoryQuery<TEntity> where TEntity : class
{
    public IIncludableRepositoryQuery<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigation)
    {
        return new IncludableRepositoryQuery<TEntity, TProperty>(Query.Include(navigation));
    }

    public new IRepositoryQuery<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return new RepositoryQuery<TEntity>(Query.Where(predicate));
    }

    public new IRepositoryQuery<TEntity> Skip(int count)
    {
        return new RepositoryQuery<TEntity>(Query.Skip(count));
    }

    public new IRepositoryQuery<TEntity> Take(int count)
    {
        return new RepositoryQuery<TEntity>(Query.Take(count));
    }

    public new IRepositoryQuery<TEntity> Distinct()
    {
        return new RepositoryQuery<TEntity>(Query.Distinct());
    }
}
