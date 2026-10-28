namespace DataRepositoryCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class RelationalRepositoryTests
{
    private static RelationalContext CreateContext(SqliteConnection connection) => new(
        new DbContextOptionsBuilder<RelationalContext>().UseSqlite(connection).Options);

    [Test]
    public async Task QueriesTranslatePredicatesAndApplyQueryFilters()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(
            new Category { Id = 1, Name = "Visible", IsActive = true },
            new Category { Id = 2, Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        ReadDataRepository<Category, int> repository = new(context);

        // Act
        string sql = repository.Where(category => category.Name.StartsWith("Vis")).ToQueryString();
        Category[] matching = await repository.Where(category => category.Name.StartsWith("Vis")).ToArrayAsync();
        int visibleCount = await repository.CountAsync();
        int allCount = await repository.IgnoreQueryFilters().CountAsync();
        Category hidden = await repository.IgnoreQueryFilters().SingleAsync(category => category.Id == 2);
        int trackedCount = context.ChangeTracker.Entries().Count();

        // Assert
        await Assert.That(sql).Contains("SELECT");
        await Assert.That(matching.Length).IsEqualTo(1);
        await Assert.That(matching[0].Name).IsEqualTo("Visible");
        await Assert.That(visibleCount).IsEqualTo(1);
        await Assert.That(allCount).IsEqualTo(2);
        await Assert.That(hidden.Name).IsEqualTo("Hidden");
        await Assert.That(trackedCount).IsEqualTo(0);
    }

    [Test]
    public async Task ExplicitlyLoadsCollectionAndReferenceNavigations()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Add(new Category
        {
            Id = 1,
            Name = "Books",
            IsActive = true,
            Products = [new Product { Id = 2, Name = "Novel" }]
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        DataRepository<Category, int> categories = new(context);
        DataRepository<Product, int> products = new(context);
        Category category = await categories.SingleAsync();
        bool collectionLoadedBefore = context.Entry(category).Collection(item => item.Products).IsLoaded;

        // Act
        await categories.LoadAsync(category, item => item.Products);
        context.ChangeTracker.Clear();
        Product product = await products.SingleAsync();
        bool referenceLoadedBefore = context.Entry(product).Reference(item => item.Category).IsLoaded;
        await products.LoadReferenceAsync(product, item => item.Category);

        // Assert
        await Assert.That(collectionLoadedBefore).IsFalse();
        await Assert.That(category.Products.Count).IsEqualTo(1);
        await Assert.That(category.Products.Single().Name).IsEqualTo("Novel");
        await Assert.That(referenceLoadedBefore).IsFalse();
        await Assert.That(context.Entry(product).Reference(item => item.Category).IsLoaded).IsTrue();
        await Assert.That(product.Category).IsNotNull();
        await Assert.That(product.Category!.Name).IsEqualTo("Books");
    }

    [Test]
    public async Task ReadRepositoryLoadsCollectionForDetachedEntity()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Add(new Category { Id = 1, Name = "Books", IsActive = true,
            Products = [new Product { Id = 2, Name = "Novel" }] });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        ReadDataRepository<Category, int> repository = new(context);
        Category category = await repository.SingleAsync();
        EntityState before = context.Entry(category).State;

        // Act
        await repository.LoadAsync(category, item => item.Products);

        // Assert
        await Assert.That(before).IsEqualTo(EntityState.Detached);
        await Assert.That(category.Products.Count).IsEqualTo(1);
        await Assert.That(category.Products.Single().Name).IsEqualTo("Novel");
        await Assert.That(context.Entry(category).State).IsEqualTo(EntityState.Detached);
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task ReadRepositoryLoadsReferenceForDetachedEntity()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Add(new Category { Id = 1, Name = "Books", IsActive = true,
            Products = [new Product { Id = 2, Name = "Novel" }] });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        ReadDataRepository<Product, int> repository = new(context);
        Product product = await repository.SingleAsync();
        EntityState before = context.Entry(product).State;

        // Act
        await repository.LoadReferenceAsync(product, item => item.Category);

        // Assert
        await Assert.That(before).IsEqualTo(EntityState.Detached);
        await Assert.That(product.Category).IsNotNull();
        await Assert.That(product.Category!.Name).IsEqualTo("Books");
        await Assert.That(context.Entry(product).State).IsEqualTo(EntityState.Detached);
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task WritesPersistAcrossContextsOnlyAfterSave()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        DataRepository<Category, int> repository = new(context);
        Category category = new() { Id = 1, Name = "Original", IsActive = true };

        // Act
        repository.Add(category);
        await using RelationalContext observer = CreateContext(connection);
        bool existsBeforeSave = await new ReadDataRepository<Category, int>(observer).AnyAsync();
        await context.SaveChangesAsync();
        category.Name = "Updated";
        await context.SaveChangesAsync();
        string persistedName = (await new ReadDataRepository<Category, int>(observer).SingleAsync()).Name;
        repository.Delete(category);
        await context.SaveChangesAsync();
        bool existsAfterDelete = await new ReadDataRepository<Category, int>(observer).AnyAsync();

        // Assert
        await Assert.That(existsBeforeSave).IsFalse();
        await Assert.That(persistedName).IsEqualTo("Updated");
        await Assert.That(existsAfterDelete).IsFalse();
    }

    public sealed class Category : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<Product> Products { get; set; } = [];
    }

    public sealed class Product : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

    private sealed class RelationalContext(DbContextOptions<RelationalContext> options) : DataContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasQueryFilter(category => category.IsActive);
            modelBuilder.Entity<Category>().HasMany(category => category.Products)
                .WithOne(product => product.Category).HasForeignKey(product => product.CategoryId);
        }
    }
}
