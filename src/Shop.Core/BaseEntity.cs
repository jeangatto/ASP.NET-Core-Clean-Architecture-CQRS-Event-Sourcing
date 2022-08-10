using System;
using Shop.Core.Interfaces;

namespace Shop.Core;

public abstract class BaseEntity : IEntityKey<Guid>
{
    protected BaseEntity() => Id = Guid.NewGuid();

    public Guid Id { get; private init; }
}