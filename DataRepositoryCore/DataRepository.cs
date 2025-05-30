﻿namespace DataRepositoryCore;

using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Repository that returns tracked entities that can be updated.
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository returns</typeparam>
/// <typeparam name="TKey">The type of the entity key</typeparam>
public class DataRepository<TEntity, TKey> : ReadDataRepository<TEntity, TKey>, IDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public DataRepository(IDataContext dataContext, ILogger<DataRepository<TEntity, TKey>> logger = null)
        : base(dataContext, logger)
    {
    }

    /// <summary>
    /// Adds the entity to the context.
    /// </summary>
    /// <param name="entity"></param>
    public void Add(TEntity entity)
    {
        Logger?.LogTrace("Adding entity id {EntityId}", entity.Id);
        DataContext.AddEntity(entity);
    }

    public override IQueryable<TEntity> AsQueryable()
    {
        Logger?.LogTrace("Returning tracked IQueryable");
        return Entities.AsQueryable();
    }

    public void Delete(TEntity entity)
    {
        Logger?.LogTrace("Deleting entity id {EntityId}", entity.Id);
        Entities.Remove(entity);
    }

    public ValueTask<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        Logger?.LogTrace("Finding entity id {EntityId}", id);
        return Entities.FindAsync(new object[] { id }, cancellationToken);
    }
}