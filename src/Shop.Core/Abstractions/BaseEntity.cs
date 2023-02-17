using System;
using System.Collections.Generic;
using Shop.Core.Interfaces;

namespace Shop.Core.Abstractions;

/// <summary>
/// Classe base que contém os comportamentos de uma entidade.
/// </summary>
public abstract class BaseEntity : IEntityKey<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// Eventos de domínio que ocorreram.
    /// </summary>
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adicionar evento de domínio.
    /// </summary>
    /// <param name="domainEvent"></param>
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Limpa os eventos de domínio.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}