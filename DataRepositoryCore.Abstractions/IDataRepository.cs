namespace DataRepositoryCore;

using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public interface IDataRepository<TEntity, TKey> : IReadDataRepository<TEntity, TKey>
    where TEntity : IEntity<TKey>
{
    void Add(TEntity entity);

    void Delete(TEntity entity);

    ValueTask<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default);
}