using System;

namespace Shop.Query.Abstractions;

/// <summary>
/// Represents the query model interface.
/// </summary>
public interface IQueryModel
{
}

/// <summary>
/// Represents the interface for a query model with a generic key type.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IQueryModel<out TKey> : IQueryModel where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the ID of the query model.
    /// </summary>
    TKey Id { get; }
}
