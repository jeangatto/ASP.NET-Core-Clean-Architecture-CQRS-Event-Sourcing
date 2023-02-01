using System;
using System.Collections.Generic;
using Shop.Core.Interfaces;

namespace Shop.Core.Abstractions;

public abstract class BaseEntity : IEntityKey<Guid>
{
    private List<IDomainEvent> _domainEvents;

    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// Eventos de domínio que ocorreram.
    /// </summary>
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    /// <summary>
    /// Adicionar evento de domínio.
    /// </summary>
    /// <param name="domainEvent"></param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Limpar os eventos de domínio.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents?.Clear();
}