namespace DataRepositoryCore;

using System.Threading;
using System.Threading.Tasks;

/// <summary>Provides tracked queries and operations that change entity state.</summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <remarks>Changes are persisted when the associated context saves changes.</remarks>
public interface IDataRepository<TEntity, TKey> : IReadDataRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>Adds an entity to the context; saving changes persists it.</summary>
    void Add(TEntity entity);

    /// <summary>Marks an entity for deletion; saving changes persists the deletion.</summary>
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
