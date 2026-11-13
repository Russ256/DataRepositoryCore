namespace DataRepositoryCore;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

/// <summary>A query retaining the navigation type for subsequent eager loading.</summary>
/// <typeparam name="TEntity">The root entity type.</typeparam>
/// <typeparam name="TProperty">The last included navigation type.</typeparam>
public interface IIncludableRepositoryQuery<TEntity, out TProperty> : IRepositoryQuery<TEntity>
    where TEntity : class
{
    /// <summary>Gets the provider responsible for extending this include chain.</summary>
    IRepositoryIncludeBuilder<TEntity> IncludeBuilder { get; }
}

/// <summary>Provider contract used by the ThenInclude extension methods.</summary>
/// <typeparam name="TEntity">The root entity type.</typeparam>
public interface IRepositoryIncludeBuilder<TEntity> where TEntity : class
{
    /// <summary>Extends an include chain from a reference navigation.</summary>
    IIncludableRepositoryQuery<TEntity, TProperty> IncludeReference<TPrevious, TProperty>(
        Expression<Func<TPrevious, TProperty>> navigation);

    /// <summary>Extends an include chain from the elements of a collection navigation.</summary>
    IIncludableRepositoryQuery<TEntity, TProperty> IncludeCollection<TPrevious, TProperty>(
        Expression<Func<TPrevious, TProperty>> navigation);
}

/// <summary>Extends eager loading to nested reference and collection navigations.</summary>
public static class RepositoryIncludeExtensions
{
    /// <summary>Includes a navigation on the previously included reference.</summary>
    public static IIncludableRepositoryQuery<TEntity, TProperty> ThenInclude<TEntity, TPrevious, TProperty>(
        this IIncludableRepositoryQuery<TEntity, TPrevious> source,
        Expression<Func<TPrevious, TProperty>> navigation) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(navigation);
        return source.IncludeBuilder.IncludeReference(navigation);
    }

    /// <summary>Includes a navigation on each element of the previously included collection.</summary>
    public static IIncludableRepositoryQuery<TEntity, TProperty> ThenInclude<TEntity, TPrevious, TProperty>(
        this IIncludableRepositoryQuery<TEntity, IEnumerable<TPrevious>> source,
        Expression<Func<TPrevious, TProperty>> navigation) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(navigation);
        return source.IncludeBuilder.IncludeCollection(navigation);
    }
}
