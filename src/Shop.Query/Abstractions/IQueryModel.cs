using System;

namespace Shop.Query.Abstractions;

/// <summary>
/// Interface marcadora para representar um modelo de query (ReadOnly).
/// </summary>
public interface IQueryModel
{
}

/// <summary>
/// Interface marcadora para representar um modelo de query (ReadOnly).
/// </summary>
/// <typeparam name="TKey">O tipo da chave.</typeparam>
public interface IQueryModel<out TKey> : IQueryModel where TKey : IEquatable<TKey>
{
    TKey Id { get; }
}