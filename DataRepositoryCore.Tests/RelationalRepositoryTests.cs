namespace DataRepositoryCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class RelationalRepositoryTests
{
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task OrderingSupportsIncludesPagingAndIndependentSortBranches(bool tracked)
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(new Category { Id = 1, Name = "A", IsActive = true },
            new Category { Id = 2, Name = "B", IsActive = true },
            new Category { Id = 3, Name = "A", IsActive = true,
                Products = [new Product { Id = 10, Name = "Novel" }] },
            new Category { Id = 4, Name = "B", IsActive = true },
            new Category { Id = 5, Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> repository = tracked
            ? new DataRepository<Category, int>(context) : new ReadDataRepository<Category, int>(context);

        // Act
        IOrderedRepositoryQuery<Category> original = repository.Query().OrderBy(item => item.Name);
        IOrderedRepositoryQuery<Category> descendingBranch = original.ThenByDescending(item => item.Id);
        int[] descendingIds = await descendingBranch.Select(item => item.Id).ToArrayAsync();
        int[] ascendingIds = await original.ThenBy(item => item.Id).Select(item => item.Id).ToArrayAsync();
        List<Category> page = await descendingBranch.Include(item => item.Products)
            .Skip(0).Take(1).ToListAsync();
        int trackedCount = context.ChangeTracker.Entries().Count();
        int[] descendingNames = await repository.Include(item => item.Products)
            .OrderByDescending(item => item.Name).ThenBy(item => item.Id)
            .Select(item => item.Id).ToArrayAsync();
        int[] replacedOrder = await descendingBranch.OrderByDescending(item => item.Id)
            .Select(item => item.Id).ToArrayAsync();
        int[] nextPage = await descendingBranch.Skip(1).Take(2).Select(item => item.Id).ToArrayAsync();

        // Assert
        await Assert.That(string.Join(",", descendingIds)).IsEqualTo("3,1,4,2");
        await Assert.That(string.Join(",", ascendingIds)).IsEqualTo("1,3,2,4");
        await Assert.That(page.Single().Id).IsEqualTo(3);
        await Assert.That(page.Single().Products.Single().Name).IsEqualTo("Novel");
        await Assert.That(trackedCount).IsEqualTo(tracked ? 2 : 0);
        await Assert.That(string.Join(",", descendingNames)).IsEqualTo("2,4,1,3");
        await Assert.That(string.Join(",", replacedOrder)).IsEqualTo("4,3,2,1");
        await Assert.That(string.Join(",", nextPage)).IsEqualTo("1,4");
    }

    [Test]
    public async Task OrderingSupportsScalarAndDtoProjectionsAndMultipleSortKeys()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(new Category { Id = 1, Name = "A", IsActive = true },
            new Category { Id = 2, Name = "B", IsActive = true },
            new Category { Id = 3, Name = "A", IsActive = true },
            new Category { Id = 4, Name = "B", IsActive = true });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> repository = new ReadDataRepository<Category, int>(context);

        // Act
        IRepositoryResultQuery<int> scalars = repository.Query().Select(item => item.Id);
        IOrderedRepositoryResultQuery<int> ordered = scalars.OrderBy(id => id % 2);
        int[] scalarIds = await ordered.ThenByDescending(id => id).ToArrayAsync();
        int[] ascendingBranch = await ordered.ThenBy(id => id).ToArrayAsync();
        int[] replaced = await ordered.OrderByDescending(id => id).ToArrayAsync();
        int[] dtoIds = await repository.Query().Select(item => new { item.Id, item.Name })
            .OrderByDescending(item => item.Name).ThenBy(item => item.Id % 2)
            .ThenByDescending(item => item.Id).Skip(1).Take(2)
            .Select(item => item.Id).ToArrayAsync();
        IOrderedRepositoryResultQuery<Category> entityAsResult = repository.Query().OrderBy(item => item.Name);
        int[] throughResultInterface = await entityAsResult.ThenByDescending(item => item.Id)
            .Select(item => item.Id).ToArrayAsync();

        // Assert
        await Assert.That(string.Join(",", scalarIds)).IsEqualTo("4,2,3,1");
        await Assert.That(string.Join(",", ascendingBranch)).IsEqualTo("2,4,1,3");
        await Assert.That(string.Join(",", replaced)).IsEqualTo("4,3,2,1");
        await Assert.That(string.Join(",", dtoIds)).IsEqualTo("2,3");
        await Assert.That(string.Join(",", throughResultInterface)).IsEqualTo("3,1,4,2");
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task QuerySupportsChecksCountsAndScalarProjection()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(new Category { Id = 1, Name = "Books", IsActive = true },
            new Category { Id = 2, Name = "Music", IsActive = true },
            new Category { Id = 3, Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> repository = new ReadDataRepository<Category, int>(context);

        // Act
        IRepositoryQuery<Category> query = repository.Query();
        IRepositoryResultQuery<int> ids = query.Select(item => item.Id);
        bool any = await ids.AnyAsync();
        bool missing = await ids.AnyAsync(id => id == 99);
        bool all = await ids.AllAsync(id => id > 0);
        int count = await ids.CountAsync();
        int matchingCount = await ids.CountAsync(id => id == 1);
        long longCount = await ids.LongCountAsync();
        long matchingLongCount = await ids.LongCountAsync(id => id == 1);
        bool contains = await ids.ContainsAsync(2);
        int sum = await ids.SumAsync(id => id);
        double average = await ids.AverageAsync(id => id);
        int min = await ids.MinAsync();
        int max = await ids.MaxAsync();
        int selectedMin = await query.MinAsync(item => item.Id);
        int selectedMax = await query.MaxAsync(item => item.Id);

        // Assert
        await Assert.That(any).IsTrue();
        await Assert.That(missing).IsFalse();
        await Assert.That(all).IsTrue();
        await Assert.That(count).IsEqualTo(2);
        await Assert.That(matchingCount).IsEqualTo(1);
        await Assert.That(longCount).IsEqualTo(2L);
        await Assert.That(matchingLongCount).IsEqualTo(1L);
        await Assert.That(contains).IsTrue();
        await Assert.That(sum).IsEqualTo(3);
        await Assert.That(average).IsEqualTo(1.5);
        await Assert.That(min).IsEqualTo(1);
        await Assert.That(max).IsEqualTo(2);
        await Assert.That(selectedMin).IsEqualTo(1);
        await Assert.That(selectedMax).IsEqualTo(2);
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task QueryCompositionSupportsDtoProjectionPagingAndIndependentBranches()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(new Category { Id = 1, Name = "Books", IsActive = true },
            new Category { Id = 2, Name = "Books", IsActive = true });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> repository = new ReadDataRepository<Category, int>(context);

        // Act
        IRepositoryQuery<Category> original = repository.Query();
        IRepositoryQuery<Category> branch = original.Where(item => item.Id == 1)
            .Skip(0).Take(1).Distinct();
        Category included = await branch.Include(item => item.Products).SingleAsync();
        CategorySummary summary = await branch.Select(item => new CategorySummary(item.Id, item.Name)).SingleAsync();
        string[] names = await original.Select(item => item.Name).Distinct().ToArrayAsync();
        List<int> ids = await original.Select(item => item.Id).Where(id => id == 2)
            .Skip(0).Take(1).Distinct().ToListAsync();
        int skippedCount = await original.Skip(1).CountAsync();
        int originalCount = await original.CountAsync();
        int streamedSum = 0;
        await foreach (int id in original.Select(item => item.Id).AsAsyncEnumerable())
        {
            streamedSum += id;
        }
        int actionSum = 0;
        await original.Select(item => item.Id).ForEachAsync(id => actionSum += id);

        // Assert
        await Assert.That(included.Id).IsEqualTo(1);
        await Assert.That(summary).IsEqualTo(new CategorySummary(1, "Books"));
        await Assert.That(names.Length).IsEqualTo(1);
        await Assert.That(names[0]).IsEqualTo("Books");
        await Assert.That(ids.Single()).IsEqualTo(2);
        await Assert.That(skippedCount).IsEqualTo(1);
        await Assert.That(originalCount).IsEqualTo(2);
        await Assert.That(streamedSum).IsEqualTo(3);
        await Assert.That(actionSum).IsEqualTo(3);
        await Assert.That(original.ToQueryString()).Contains("SELECT");
    }

    [Test]
    public async Task QueryPreservesEmptyAndMultipleResultSemanticsAndCancellation()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(new Category { Id = 1, Name = "Books", IsActive = true },
            new Category { Id = 2, Name = "Music", IsActive = true });
        await context.SaveChangesAsync();
        IReadDataRepository<Category, int> repository = new ReadDataRepository<Category, int>(context);
        IRepositoryResultQuery<int> ids = repository.Query().Select(item => item.Id);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        // Act
        int first = await ids.Where(id => id == 1).FirstAsync();
        int matchingFirst = await ids.FirstAsync(id => id == 2);
        int firstDefault = await ids.Where(id => id == 99).FirstOrDefaultAsync();
        int matchingFirstDefault = await ids.FirstOrDefaultAsync(id => id == 99);
        int singleDefault = await ids.Where(id => id == 99).SingleOrDefaultAsync();
        int matchingSingleDefault = await ids.SingleOrDefaultAsync(id => id == 99);
        Category? missing = await repository.Query().FirstOrDefaultAsync(item => item.Id == 99);

        // Assert
        await Assert.That(first).IsEqualTo(1);
        await Assert.That(matchingFirst).IsEqualTo(2);
        await Assert.That(firstDefault).IsEqualTo(0);
        await Assert.That(matchingFirstDefault).IsEqualTo(0);
        await Assert.That(singleDefault).IsEqualTo(0);
        await Assert.That(matchingSingleDefault).IsEqualTo(0);
        await Assert.That(missing).IsNull();
        await Assert.That(async () => await ids.SingleAsync()).Throws<System.InvalidOperationException>();
        await Assert.That(async () => await ids.Where(id => id == 99).FirstAsync()).Throws<System.InvalidOperationException>();
        await Assert.That(async () => await ids.CountAsync(cancellation.Token)).Throws<System.OperationCanceledException>();
        await Assert.That(async () => await ids.ToListAsync(cancellation.Token)).Throws<System.OperationCanceledException>();
    }

    public sealed record CategorySummary(int Id, string Name);

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task ThenIncludeLoadsNestedReferencesAndCollectionsWithRepositoryTracking(bool tracked)
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Add(new Category { Id = 1, Name = "Books", IsActive = true,
            Products = [new Product { Id = 2, Name = "Novel", Manufacturer = new Manufacturer
            {
                Id = 3, Country = new Country { Id = 4, Name = "UK",
                    Offices = [new Office { Id = 5, Rooms = [new Room { Id = 6, Name = "Reading room" }] }] }
            } }] });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> categories = tracked
            ? new DataRepository<Category, int>(context) : new ReadDataRepository<Category, int>(context);
        IReadDataRepository<Product, int> products = tracked
            ? new DataRepository<Product, int>(context) : new ReadDataRepository<Product, int>(context);

        // Act
        IIncludableRepositoryQuery<Category, List<Product>> original = categories.Include(item => item.Products);
        Category nested = await original.ThenInclude(item => item.Manufacturer)
            .ThenInclude(item => item!.Country).Where(item => item.Id == 1).SingleAsync();
        int trackedCount = context.ChangeTracker.Entries().Count();
        context.ChangeTracker.Clear();
        Category shallow = await original.SingleAsync();
        context.ChangeTracker.Clear();
        Product referenceThenCollection = await products.Include(item => item.Manufacturer)
            .ThenInclude(item => item!.Country).ThenInclude(item => item!.Offices)
            .ThenInclude(item => item.Rooms).Include(item => item.Category)
            .SingleAsync(item => item.Id == 2);

        // Assert
        await Assert.That(nested.Products.Single().Manufacturer!.Country!.Name).IsEqualTo("UK");
        await Assert.That(trackedCount).IsEqualTo(tracked ? 4 : 0);
        await Assert.That(shallow.Products.Single().Manufacturer).IsNull();
        await Assert.That(referenceThenCollection.Manufacturer!.Country!.Offices.Single()
            .Rooms.Single().Name).IsEqualTo("Reading room");
        await Assert.That(referenceThenCollection.Category!.Name).IsEqualTo("Books");
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(tracked ? 6 : 0);
    }

    [Test]
    public async Task IncludeLoadsNavigationsWithoutTrackingOrChangingOtherQueries()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        await using RelationalContext context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.AddRange(
            new Category { Id = 1, Name = "Books", IsActive = true,
                Products = [new Product { Id = 2, Name = "Novel" }] },
            new Category { Id = 3, Name = "Music", IsActive = true },
            new Category { Id = 4, Name = "Hidden", IsActive = false });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        IReadDataRepository<Category, int> categories = new ReadDataRepository<Category, int>(context);
        IReadDataRepository<Product, int> products = new ReadDataRepository<Product, int>(context);

        // Act
        IRepositoryQuery<Category> query = categories.Include(category => category.Products);
        IRepositoryQuery<Category> filtered = query.Where(category => category.Id == 1);
        Category category = await filtered.SingleAsync();
        Category[] all = await query.ToArrayAsync();
        List<Category> list = await filtered.ToListAsync();
        Product product = await products.Include(item => item.Category)
            .Include(item => item.Category).SingleAsync(item => item.Id == 2);
        Category plain = await categories.SingleAsync(item => item.Id == 1);
        Category? missing = await query.SingleOrDefaultAsync(item => item.Id == 99);
        Category? filteredSingle = await filtered.SingleOrDefaultAsync();

        // Assert
        await Assert.That(category.Products.Single().Name).IsEqualTo("Novel");
        await Assert.That(product.Category!.Name).IsEqualTo("Books");
        await Assert.That(all.Length).IsEqualTo(2);
        await Assert.That(list.Count).IsEqualTo(1);
        await Assert.That(plain.Products.Count).IsEqualTo(0);
        await Assert.That(missing).IsNull();
        await Assert.That(filteredSingle!.Id).IsEqualTo(1);
        await Assert.That(context.ChangeTracker.Entries().Count()).IsEqualTo(0);
    }

    [Test]
    public async Task IncludePreservesWriteRepositoryTrackingAndPersistsNavigationChanges()
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
        IDataRepository<Category, int> repository = new DataRepository<Category, int>(context);

        // Act
        Category category = await repository.Include(item => item.Products).SingleAsync();
        int trackedCount = context.ChangeTracker.Entries().Count();
        category.Products.Single().Name = "Updated";
        await context.SaveChangesAsync();
        await using RelationalContext observer = CreateContext(connection);
        Product persisted = await new ReadDataRepository<Product, int>(observer).SingleAsync();

        // Assert
        await Assert.That(trackedCount).IsEqualTo(2);
        await Assert.That(persisted.Name).IsEqualTo("Updated");
    }

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
    public async Task IgnoreAutoIncludesSuppressesConfiguredNavigation()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        DbContextOptions<AutoIncludeContext> options = new DbContextOptionsBuilder<AutoIncludeContext>()
            .UseSqlite(connection).Options;
        await using AutoIncludeContext context = new(options);
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
        ReadDataRepository<Category, int> repository = new(context);

        // Act
        Category included = await repository.SingleAsync();
        Category ignored = await repository.IgnoreAutoIncludes().SingleAsync();

        // Assert
        await Assert.That(included.Products.Count).IsEqualTo(1);
        await Assert.That(included.Products.Single().Name).IsEqualTo("Novel");
        await Assert.That(ignored.Products.Count).IsEqualTo(0);
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
        public int? ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }
    }

    public sealed class Manufacturer : IEntity<int>
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
        public List<Product> Products { get; set; } = [];
    }

    public sealed class Country : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Manufacturer> Manufacturers { get; set; } = [];
        public List<Office> Offices { get; set; } = [];
    }

    public sealed class Office : IEntity<int>
    {
        public int Id { get; set; }
        public List<Room> Rooms { get; set; } = [];
    }

    public sealed class Room : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class AutoIncludeContext(DbContextOptions<AutoIncludeContext> options) : DataContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasMany(category => category.Products)
                .WithOne(product => product.Category).HasForeignKey(product => product.CategoryId);
            modelBuilder.Entity<Category>().Navigation(category => category.Products).AutoInclude();
        }
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
