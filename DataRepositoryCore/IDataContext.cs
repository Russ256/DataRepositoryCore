namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Provides the EF Core context operations required by repositories.</summary>
public interface IDataContext
{
    /// <summary>Gets access to database-related operations.</summary>
    DatabaseFacade Database { get; }

    /// <summary>Adds an entity to the context for insertion when changes are saved.</summary>
    void AddEntity<TEntity>(TEntity entity) where TEntity : class, IEntity;

    /// <summary>Gets the change-tracking entry for an entity.</summary>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>Gets the entity set for the specified entity type.</summary>
    DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IEntity;

    /// <summary>Saves context changes and returns the number of state entries written.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
