namespace DataRepositoryCore;

using Microsoft.EntityFrameworkCore;

/// <summary>Base EF Core context implementing repository context operations.</summary>
/// <param name="options">The options used to configure the context.</param>
public abstract class DataContext(DbContextOptions options) : DbContext(options), IDataContext
{
    /// <inheritdoc />
    public void AddEntity<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        base.Add(entity);

    }

    /// <inheritdoc />
    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return Set<T>();
    }
}
