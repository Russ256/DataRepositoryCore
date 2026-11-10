namespace DataRepositoryCore;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

public class DependencyInjectionTests
{
    [Test]
    public async Task AddDbContextSharesScopedContextWithBothRepositories()
    {
        // Arrange
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();
        ContextRecorder recorder = new();
        ServiceCollection services = new();
        services.AddDbContext<IDataContext, RegistrationContext>(options =>
            options.UseSqlite(connection).AddInterceptors(recorder));
        services.AddDataRepositories();
        await using ServiceProvider provider = services.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IDataContext context = scope.ServiceProvider.GetRequiredService<IDataContext>();
        IReadDataRepository<Item, int> read = scope.ServiceProvider.GetRequiredService<IReadDataRepository<Item, int>>();
        IDataRepository<Item, int> write = scope.ServiceProvider.GetRequiredService<IDataRepository<Item, int>>();
        await context.Database.EnsureCreatedAsync();
        write.Add(new Item { Id = 1, Value = 42 });
        await context.SaveChangesAsync();
        recorder.Contexts.Clear();

        // Act
        Item readItem = await read.SingleAsync();
        Item writeItem = await write.SingleAsync();
        await using AsyncServiceScope otherScope = provider.CreateAsyncScope();
        IDataContext otherContext = otherScope.ServiceProvider.GetRequiredService<IDataContext>();

        // Assert
        await Assert.That(readItem.Value).IsEqualTo(42);
        await Assert.That(writeItem.Value).IsEqualTo(42);
        await Assert.That(recorder.Contexts.Count).IsEqualTo(2);
        await Assert.That(ReferenceEquals(recorder.Contexts[0], context)).IsTrue();
        await Assert.That(ReferenceEquals(recorder.Contexts[1], context)).IsTrue();
        await Assert.That(ReferenceEquals(context, otherContext)).IsFalse();
    }

    private sealed class ContextRecorder : DbCommandInterceptor
    {
        public List<DbContext?> Contexts { get; } = [];

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Contexts.Add(eventData.Context);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
    }

    public sealed class Item : IEntity<int>
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public sealed class RegistrationContext(DbContextOptions<RegistrationContext> options) : DataContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
    }
}
