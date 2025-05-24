namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;

public abstract class DataContext(DbContextOptions options) : DbContext(options), IDataContext
{
    public void AddEntity<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        base.Add(entity);

    }

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return Set<T>();
    }
}