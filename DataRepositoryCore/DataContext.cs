namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;

public abstract class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return this.Set<T>();
    }

    public void AddEntity<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        base.Add(entity);
    }
}