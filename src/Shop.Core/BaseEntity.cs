using System;
using Shop.Core.Interfaces;

namespace Shop.Core;

public abstract class BaseEntity : IEntityKey<Guid>
{
    public Guid Id { get; private init; } = Guid.NewGuid();
}