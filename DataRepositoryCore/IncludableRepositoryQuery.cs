namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

internal sealed class IncludableRepositoryQuery<TEntity, TNavigation>(IIncludableQueryable<TEntity, TNavigation> query)
    : RepositoryQuery<TEntity>(query), IIncludableRepositoryQuery<TEntity, TNavigation>, IRepositoryIncludeBuilder<TEntity>
    where TEntity : class
{
    public IRepositoryIncludeBuilder<TEntity> IncludeBuilder => this;

    public IIncludableRepositoryQuery<TEntity, TProperty> IncludeReference<TPrevious, TProperty>(
        Expression<Func<TPrevious, TProperty>> navigation)
    {
        IIncludableQueryable<TEntity, TPrevious> source = (IIncludableQueryable<TEntity, TPrevious>)query;
        return new IncludableRepositoryQuery<TEntity, TProperty>(source.ThenInclude(navigation));
    }

    public IIncludableRepositoryQuery<TEntity, TProperty> IncludeCollection<TPrevious, TProperty>(
        Expression<Func<TPrevious, TProperty>> navigation)
    {
        IIncludableQueryable<TEntity, IEnumerable<TPrevious>> source =
            (IIncludableQueryable<TEntity, IEnumerable<TPrevious>>)query;
        return new IncludableRepositoryQuery<TEntity, TProperty>(source.ThenInclude(navigation));
    }
}
