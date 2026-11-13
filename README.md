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

Use `LoadAsync(entity, item => item.Children)` for a collection navigation or `LoadReferenceAsync(entity, item => item.Parent)` for a reference navigation, substituting navigation properties defined on your entity. With the default EF Core implementation, these methods can load navigations on a detached entity returned by the read repository without first attaching it; the entity and loaded data remain untracked. The repository's context must still be alive, and EF Core query filters apply to the load. If you need to save changes to those entities, use tracked queries or explicitly attach them to the context.

`Add` and `Delete` change the context's entity state; they do not save to the database. Call `IDataContext.SaveChangesAsync` to persist those operations and changes to tracked entities.

`FindAsync` is available on `IDataRepository`. It looks up an entity by its EF Core primary key and returns `null` when no entity is found. The entity must have a single-property primary key whose type matches `TKey`; composite primary keys are not supported. EF Core may return an entity already tracked by the context without querying the database.

## Eager loading without EF Core references

Application services can reference only `DataRepositoryCore.Abstractions` and use `Include` with asynchronous query methods:

```csharp
User user = await users
    .Include(user => user.Tenant)
    .SingleAsync(user => user.Id == userId, cancellationToken);
```

Here `users` is an `IReadDataRepository<User, Guid>` (use your entity's key type). `Include` accepts reference and collection navigations and returns `IIncludableRepositoryQuery<TEntity, TProperty>`, which also supports all `IRepositoryQuery<TEntity>` operations. Use `ThenInclude` immediately after `Include` or another `ThenInclude` to load nested navigations:

```csharp
User user = await users
    .Include(user => user.Tenant)
        .ThenInclude(tenant => tenant.Settings)
    .Include(user => user.Roles)
        .ThenInclude(role => role.Permissions)
    .SingleAsync(user => user.Id == userId, cancellationToken);
```

For collections, the `ThenInclude` lambda receives an element. Chain further `Include` or `Where` calls, then execute with `SingleAsync`, `SingleOrDefaultAsync`, `ToListAsync`, or `ToArrayAsync`. Both single-result methods also accept a predicate. Each composition call returns a new query without changing the repository or earlier queries. These methods require only the `DataRepositoryCore` namespace, with no EF Core imports.

Queries preserve the repository's tracking behavior and configured query filters. Execute them while the repository context is alive. The infrastructure project that registers the context still needs `DataRepositoryCore` and an EF Core provider; application services using these abstractions need no EF Core types or extension methods.

Use `Query()` to start a query without an include:

```csharp
int count = await users.Query()
    .Where(user => user.IsActive)
    .CountAsync(cancellationToken);

List<string> names = await users.Query()
    .Select(user => user.Name)
    .Distinct()
    .ToListAsync(cancellationToken);
```

The query interfaces support `AnyAsync`, `AllAsync`, `CountAsync`, `LongCountAsync`, `ContainsAsync`, `FirstAsync`, `FirstOrDefaultAsync`, `SingleAsync`, `SingleOrDefaultAsync`, `ToListAsync`, `ToArrayAsync`, `ForEachAsync`, and `AsAsyncEnumerable`. Checks, counts, and first/single methods accept predicates where applicable. Async execution methods accept cancellation tokens; use `WithCancellation(token)` when enumerating `AsAsyncEnumerable()`.

Compose queries with `Where`, `Skip`, `Take`, and `Distinct`. `Select` returns `IRepositoryResultQuery<TResult>`, supporting value types, nullable values, anonymous types, and DTOs. This result interface supports further composition and execution, but does not expose includes. `MinAsync` and `MaxAsync` support both direct results and selectors; `SumAsync` and `AverageAsync` accept selectors for `int`, `long`, `float`, `double`, `decimal`, and their nullable forms. For example, a projected integer query can use `SumAsync(value => value, cancellationToken)`.

Use `OrderBy` or `OrderByDescending` followed by `ThenBy` or `ThenByDescending` for additional sort keys. Ordering works on entity queries and after projection into scalar values or DTOs:

```csharp
List<User> page = await users.Query()
    .OrderBy(user => user.LastName)
    .ThenBy(user => user.Id)
    .Include(user => user.Tenant)
    .Skip(20)
    .Take(10)
    .ToListAsync(cancellationToken);
```

Entity ordering returns `IOrderedRepositoryQuery<TEntity>`; projected ordering returns `IOrderedRepositoryResultQuery<TResult>`. `ThenBy` methods are available on these ordered interfaces. Apply additional sort keys before further composition such as `Include`, `Where`, or `Skip`. Calling `OrderBy` again replaces previous sorting. Each sorting call creates a new query, preserving other branches. For deterministic paging, order before `Skip`/`Take` and end with a unique sort key such as `Id`.

Database execution uses asynchronous methods; synchronous execution methods such as `Count()` are not exposed. Translation and aggregate support depend on the configured EF Core provider. Scalar projections do not track entities; projections containing entities retain the source query's tracking behavior. `ToQueryString()` is available for diagnostics.

## Building and testing

Install a compatible .NET 11 SDK, then run `dotnet build DataRepositoryCore.sln` and `dotnet test DataRepositoryCore.sln` from the repository root.

To verify the generated NuGet packages from an isolated consumer, run `pwsh -NoProfile -File PackageSmokeTest/run.ps1` (requires PowerShell 7 and access to the configured NuGet feeds). The script packs both libraries, checks their README and XML documentation, and restores and runs a sample against the freshly built packages. The consumer is created under the repository's ignored `obj/PackageSmokeTest/` directory, so it inherits repository NuGet configuration. The script prints the artifact directory for inspection.
