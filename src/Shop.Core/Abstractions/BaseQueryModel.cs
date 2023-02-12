using System;
using Shop.Core.Interfaces;

namespace Shop.Core.Abstractions;

public abstract class BaseQueryModel : IQueryModel
{
    public Guid Id { get; private init; } = Guid.NewGuid();
}