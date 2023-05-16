using System;
using System.Collections.Generic;
using Shop.Core.Events;

namespace Shop.Core.Domain;

/// <summary>
/// Classe base que contém os comportamentos de uma entidade.
/// </summary>
public abstract class Entity : IEntity<Guid>
{
    private readonly List<Event> _domainEvents = new();

    /// <summary>
    /// Eventos de domínio que ocorreram.
    /// </summary>
    public IEnumerable<Event> DomainEvents => _domainEvents.AsReadOnly();

    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Adicionar evento de domínio.
    /// </summary>
    /// <param name="domainEvent"></param>
    protected void AddDomainEvent(Event domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Limpa os eventos de domínio.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}