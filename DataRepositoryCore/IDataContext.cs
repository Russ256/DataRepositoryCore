namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

public interface IDataContext
{
    DatabaseFacade Database { get; }

    void AddEntity<TEntity>(TEntity entity) where TEntity : class, IEntity;

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class, IEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}