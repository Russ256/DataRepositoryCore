namespace DataRepositoryCore;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Registers repository services with dependency injection.</summary>
public static class ServicesExtension
{
    /// <summary>
    /// Adds generic repositories.  
    /// Note: An implementation of IDataContext needs to be added to the service collection for these to work.
    /// </summary>
    /// <param name="services">The collection to which scoped repositories are added.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IDataRepository<,>), typeof(DataRepository<,>));
        services.AddScoped(typeof(IReadDataRepository<,>), typeof(ReadDataRepository<,>));

        return services;
    }
}
