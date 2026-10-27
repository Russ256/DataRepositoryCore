namespace DataRepositoryCore;

using System.Threading;
using System.Threading.Tasks;

public interface IDataRepository<TEntity, TKey> : IReadDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    void Add(TEntity entity);

    void Delete(TEntity entity);

    /// <summary>
    /// Finds an entity by its single-property Entity Framework primary key.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity, or null if it is not found.</returns>
    /// <remarks>
    /// The entity must have a single-property primary key whose type matches <typeparamref name="TKey" />.
    /// Composite primary keys are not supported by this method.
    /// </remarks>
    ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}
