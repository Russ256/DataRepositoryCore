namespace DataRepositoryCore;

using Microsoft.Extensions.DependencyInjection;

public static class ServicesExtension
{
    /// <summary>
    /// Adds generic repositories.  
    /// Note: An implementation of IDataContext needs to be added to the service collection for these to work.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IDataRepository<,>), typeof(DataRepository<,>));
        services.AddScoped(typeof(IReadDataRepository<,>), typeof(ReadDataRepository<,>));

        return services;
    }
}