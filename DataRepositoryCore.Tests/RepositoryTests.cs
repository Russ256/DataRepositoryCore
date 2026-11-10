namespace DataRepositoryCore;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class RepositoryTests
{
    private static TestContext CreateContext() => new(
        new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Test]
    public async Task ConstructorRejectsNullContext()
    {
        // Act
        ArgumentNullException? error = await Assert.That(() => new ReadDataRepository<Item, int>(null!)).Throws<ArgumentNullException>();

        // Assert
        await Assert.That(error!.ParamName).IsEqualTo("dataContext");
    }

    [Test]
    public async Task ReadQueriesFilterWithoutTracking()
    {
        // Arrange
        await using TestContext context = CreateContext();
        context.AddRange(new Item { Id = 1, Value = 10 }, new Item { Id = 2, Value = 20 });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        ReadDataRepository<Item, int> repository = new(context);

        // Act
        bool hasTwenty = await repository.AnyAsync(x => x.Value == 20);
        bool hasThirty = await repository.AnyAsync(x => x.Value == 30);
        int count = await repository.CountAsync();
        int filteredCount = await repository.CountAsync(x => x.Value > 10);
        Item single = await repository.SingleAsync(x => x.Value == 20);
        int filteredResults = (await repository.WhereAsync(x => x.Value > 10)).Count;
        int arrayLength = (await repository.ToArrayAsync()).Length;
        int trackedCount = context.ChangeTracker.Entries().Count();

        // Assert
        await Assert.That(hasTwenty).IsTrue();
        await Assert.That(hasThirty).IsFalse();
        await Assert.That(count).IsEqualTo(2);
        await Assert.That(filteredCount).IsEqualTo(1);
        await Assert.That(single.Id).IsEqualTo(2);
        await Assert.That(filteredResults).IsEqualTo(1);
        await Assert.That(arrayLength).IsEqualTo(2);
        await Assert.That(trackedCount).IsEqualTo(0);
    }

    [Test]
    public async Task AggregateQueriesReturnExpectedValues()
    {
        // Arrange
        await using TestContext context = CreateContext();
        context.AddRange(new Item { Id = 1, Value = 10 }, new Item { Id = 2, Value = 20 });
        await context.SaveChangesAsync();
        ReadDataRepository<Item, int> repository = new(context);

        // Act
        double average = await repository.AverageAsync(x => x.Value);
        int sum = await repository.SumAsync(x => x.Value);
        int min = await repository.MinAsync(x => x.Value);
        int max = await repository.MaxAsync(x => x.Value);

        // Assert
        await Assert.That(average).IsEqualTo(15d);
        await Assert.That(sum).IsEqualTo(30);
        await Assert.That(min).IsEqualTo(10);
        await Assert.That(max).IsEqualTo(20);
    }

    [Test]
    public async Task EmptyQueriesReturnNullOrThrowAsAppropriate()
    {
        // Arrange
        await using TestContext context = CreateContext();
        ReadDataRepository<Item, int> repository = new(context);

        // Act
        Item? first = await repository.FirstOrDefaultAsync();
        Item? single = await repository.SingleOrDefaultAsync();

        // Assert
        await Assert.That(first).IsNull();
        await Assert.That(single).IsNull();
        await Assert.That(async () => await repository.FirstAsync()).Throws<InvalidOperationException>();
        await Assert.That(async () => await repository.SingleAsync()).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task SingleQueriesRejectMultipleMatches()
    {
        // Arrange
        await using TestContext context = CreateContext();
        context.AddRange(new Item { Id = 1, Value = 10 }, new Item { Id = 2, Value = 10 });
        await context.SaveChangesAsync();
        ReadDataRepository<Item, int> repository = new(context);

        // Act and assert
        await Assert.That(async () => await repository.SingleAsync(x => x.Value == 10)).Throws<InvalidOperationException>();
        await Assert.That(async () => await repository.SingleOrDefaultAsync(x => x.Value == 10)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task QueryHonorsCancelledToken()
    {
        // Arrange
        await using TestContext context = CreateContext();
        ReadDataRepository<Item, int> repository = new(context);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        // Act and assert
        await Assert.That(async () => await repository.ToListAsync(cancellation.Token)).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task FindReturnsAlreadyTrackedInstance()
    {
        // Arrange
        await using TestContext context = CreateContext();
        Item item = new() { Id = 1, Value = 10 };
        context.Add(item);
        await context.SaveChangesAsync();
        item.Value = 20;
        DataRepository<Item, int> repository = new(context);

        // Act
        Item? found = await repository.FindAsync(1);

        // Assert
        await Assert.That(ReferenceEquals(item, found)).IsTrue();
        await Assert.That(found!.Value).IsEqualTo(20);
    }

    [Test]
    public async Task AddTracksEntityAsAdded()
    {
        // Arrange
        await using TestContext context = CreateContext();
        DataRepository<Item, int> repository = new(context);
        Item item = new() { Id = 1, Value = 10 };

        // Act
        repository.Add(item);

        // Assert
        await Assert.That(context.Entry(item).State).IsEqualTo(EntityState.Added);
    }

    [Test]
    public async Task FindReturnsTrackedEntityOrNull()
    {
        // Arrange
        await using TestContext context = CreateContext();
        context.Add(new Item { Id = 1, Value = 10 });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        DataRepository<Item, int> repository = new(context);

        // Act
        Item? found = await repository.FindAsync(1);
        Item? missing = await repository.FindAsync(99);

        // Assert
        await Assert.That(found).IsNotNull();
        await Assert.That(found!.Id).IsEqualTo(1);
        await Assert.That(missing).IsNull();
        await Assert.That(context.Entry(found).State).IsEqualTo(EntityState.Unchanged);
    }

    [Test]
    public async Task WritableQueryTracksChangesForSaving()
    {
        // Arrange
        await using TestContext context = CreateContext();
        context.Add(new Item { Id = 1, Value = 10 });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        DataRepository<Item, int> repository = new(context);

        // Act
        Item queried = await repository.SingleAsync();
        EntityState queryState = context.Entry(queried).State;
        queried.Value = 25;
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        Item reloaded = await repository.SingleAsync();

        // Assert
        await Assert.That(queryState).IsEqualTo(EntityState.Unchanged);
        await Assert.That(reloaded.Value).IsEqualTo(25);
    }

    [Test]
    public async Task DeleteMarksEntityDeletedAndRemovesItWhenSaved()
    {
        // Arrange
        await using TestContext context = CreateContext();
        Item item = new() { Id = 1, Value = 10 };
        context.Add(item);
        await context.SaveChangesAsync();
        DataRepository<Item, int> repository = new(context);

        // Act
        repository.Delete(item);
        EntityState deleteState = context.Entry(item).State;
        await context.SaveChangesAsync();
        bool any = await repository.AnyAsync();

        // Assert
        await Assert.That(deleteState).IsEqualTo(EntityState.Deleted);
        await Assert.That(any).IsFalse();
    }

    [Test]
    public async Task RegistersRepositoriesAsScopedServices()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        IServiceCollection returnedServices = services.AddDataRepositories();
        services.AddScoped<IDataContext>(_ => CreateContext());
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        IReadDataRepository<Item, int> read = scope.ServiceProvider.GetRequiredService<IReadDataRepository<Item, int>>();
        IDataRepository<Item, int> write = scope.ServiceProvider.GetRequiredService<IDataRepository<Item, int>>();
        IReadDataRepository<Item, int> sameScopeRead = scope.ServiceProvider.GetRequiredService<IReadDataRepository<Item, int>>();
        using IServiceScope otherScope = provider.CreateScope();
        IReadDataRepository<Item, int> otherScopeRead = otherScope.ServiceProvider.GetRequiredService<IReadDataRepository<Item, int>>();

        // Assert
        await Assert.That(ReferenceEquals(services, returnedServices)).IsTrue();
        await Assert.That(read).IsTypeOf<ReadDataRepository<Item, int>>();
        await Assert.That(write).IsTypeOf<DataRepository<Item, int>>();
        await Assert.That(ReferenceEquals(read, sameScopeRead)).IsTrue();
        await Assert.That(ReferenceEquals(read, otherScopeRead)).IsFalse();
    }

    public sealed class Item : IEntity<int>
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    private sealed class TestContext(DbContextOptions<TestContext> options) : DataContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Item>();
    }
}
