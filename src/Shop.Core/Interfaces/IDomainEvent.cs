using System;
using MediatR;

namespace Shop.Core.Interfaces;

/// <summary>
/// Interface marcadora para representar um evento domínio.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Quando ocorreu o evento.
    /// </summary>
    DateTime OccurredOn { get; }
}