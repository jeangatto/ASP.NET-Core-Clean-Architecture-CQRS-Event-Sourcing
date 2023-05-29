using System;
using System.Collections.Generic;

namespace Shop.Core.Shared;

/// <summary>
/// Classe base que contém os comportamentos de uma entidade.
/// </summary>
public abstract class BaseEntity : IEntity<Guid>
{
    private readonly List<BaseEvent> _domainEvents = new();

    /// <summary>
    /// Eventos de domínio que ocorreram.
    /// </summary>
    public IEnumerable<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Adicionar evento de domínio.
    /// </summary>
    /// <param name="domainEvent"></param>
    protected void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Limpa os eventos de domínio.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}