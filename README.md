# DataRepositoryCore

Generic repositories for Entity Framework Core. 

## Setup

Reference `DataRepositoryCore` and an EF Core database provider in your application. Define an entity that implements `IEntity<TKey>` and a context that derives from `DataContext` (or implements `IDataContext` directly):

```csharp
using System;
using DataRepositoryCore;
using Microsoft.EntityFrameworkCore;

public sealed class Customer : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class AppDataContext(DbContextOptions<AppDataContext> options)
    : DataContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
}
```

Register the context as `IDataContext` and add the repositories. For example, with the `Microsoft.EntityFrameworkCore.SqlServer` provider:

```csharp
using DataRepositoryCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();
string connectionString = "<your SQL Server connection string>";

services.AddDbContext<IDataContext, AppDataContext>(options =>
    options.UseSqlServer(connectionString));
services.AddDataRepositories();
```

Replace the placeholder with configuration appropriate for your application. Register the context and repositories in the same dependency injection scope so they share the context. Repositories are registered as scoped services.

## Reading and writing

Inject `IReadDataRepository<Customer, Guid>` for read queries, or `IDataRepository<Customer, Guid>` for tracked queries and writes. For example, in a scoped service:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using DataRepositoryCore;

public sealed class CustomerService(
    IDataRepository<Customer, Guid> customers,
    IDataContext context)
{
    public async Task<Guid> CreateAsync(string name, CancellationToken cancellationToken)
    {
        Customer customer = new() { Id = Guid.NewGuid(), Name = name };
        customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }

    public async Task RenameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        Customer? customer = await customers.FindAsync(id, cancellationToken);
        if (customer is null)
        {
            return;
        }

        customer.Name = name;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken)
    {
        customers.Delete(customer);
        await context.SaveChangesAsync(cancellationToken);
    }
}
```

`IReadDataRepository` queries are untracked by default: changing a returned entity does not cause that change to be saved automatically. `IDataRepository` queries are tracked by default, so changes to returned entities can be saved through the same context. Queryables remain subject to EF Core query composition and tracking options.

`Add` and `Delete` change the context's entity state; they do not save to the database. Call `IDataContext.SaveChangesAsync` to persist those operations and changes to tracked entities.

`FindAsync` is available on `IDataRepository`. It looks up an entity by its EF Core primary key and returns `null` when no entity is found. The entity must have a single-property primary key whose type matches `TKey`; composite primary keys are not supported. EF Core may return an entity already tracked by the context without querying the database.

## Building and testing

Install a compatible .NET 11 SDK, then run `dotnet build DataRepositoryCore.sln` and `dotnet test DataRepositoryCore.sln` from the repository root.
