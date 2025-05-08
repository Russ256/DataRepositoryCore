# DataRepositoryCore
Generic Data Repositories for Entity Framework Core

# Summary
There are two fully generic repositories implemented:
1. IReadDataRepository - For non tracked entities, use this for faster performance when updates not required.
1. IDataRepository - For tracked entities.

# Instructions
1. Implement IEntity<TKey> on your entities.
1. Implement IDataContext on your DbContext or derive your context from DataContext.
1. Add the default repositories to the services container using the extension mehthod AddDataRepositories.
1. Make sure you use the IDataContext interface when adding your EF context:
```
    services.AddEntityFrameworkSqlServer()
        .AddDbContext<IDataContext, YourDataContext>(options => .....);
```
1. Inject into you code:
```
     public YourClassConstructor(IDataRepository<Customer, Guid> repository)
```
