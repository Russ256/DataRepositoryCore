using System;
using System.Threading.Tasks;
using DataRepositoryCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();
services.AddDbContext<IDataContext, ConsumerContext>(options =>
    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
services.AddDataRepositories();
await using ServiceProvider provider = services.BuildServiceProvider();
await using AsyncServiceScope scope = provider.CreateAsyncScope();
IDataRepository<Customer, int> repository = scope.ServiceProvider.GetRequiredService<IDataRepository<Customer, int>>();
IDataContext context = scope.ServiceProvider.GetRequiredService<IDataContext>();
repository.Add(new Customer { Id = 1, Name = "Example" });
await context.SaveChangesAsync();
IReadDataRepository<Customer, int> reader = scope.ServiceProvider.GetRequiredService<IReadDataRepository<Customer, int>>();
Customer customer = await reader.SingleAsync();
if (customer.Name != "Example")
{
    throw new InvalidOperationException("The packaged repository did not return the saved entity.");
}

public sealed class Customer : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class ConsumerContext(DbContextOptions<ConsumerContext> options) : DataContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
}
