# DataRepositoryCore
Generic Data Repositories for Entity Framework Code

# Summary
There two repositories implemented:
1. IReadDataRepository - For non tracked entities, use for faster performance when updates not required.
1. IDataRepository - For tracked entities.

# Instructions
1. Implement IEntity<TKey> on your entities.
1. Implement IDataContext on your DbContext or derive your context from DataContext.
1. Add your implementation of IDataContext to the services container.
1. Add the default repositories to the services container using the extension mehthod AddDataRepositories.
