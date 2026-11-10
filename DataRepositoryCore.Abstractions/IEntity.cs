namespace DataRepositoryCore;

/// <summary>Marks an entity that can be used with a data context.</summary>
public interface IEntity
{
}

/// <summary>Represents an entity with a typed identifier.</summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public interface IEntity<TKey> : IEntity
{
    /// <summary>Gets the entity identifier.</summary>
    TKey Id { get; }
}
