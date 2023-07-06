using System;

namespace Shop.Core.SharedKernel;

/// <summary>
/// This is the base interface for all entities
/// </summary>
public interface IEntity
{
}

public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the ID of the entity.
    /// </summary>
    TKey Id { get; }
}