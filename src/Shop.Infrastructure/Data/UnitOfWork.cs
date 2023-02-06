using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Core.Abstractions;
using Shop.Core.Events;
using Shop.Core.Extensions;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Events;

namespace Shop.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopContext _shopContext;
    private readonly EventContext _eventContext;
    private readonly IMediator _mediator;

    public UnitOfWork(ShopContext shopContext, EventContext eventContext, IMediator mediator)
    {
        _shopContext = shopContext;
        _eventContext = eventContext;
        _mediator = mediator;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _shopContext
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = new List<IDomainEvent>();
        var storedEvents = new List<StoredEvent>();

        if (domainEntities.Any())
        {
            domainEvents = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            foreach (var @event in domainEvents)
            {
                var type = @event.GetGenericTypeName();
                var data = @event.ToJson();
                storedEvents.Add(new StoredEvent(type, data));
            }

            // Limpando os eventos das entidades.
            domainEntities
                .ForEach(entry => entry.Entity.ClearDomainEvents());
        }

        await _shopContext.SaveChangesAsync(cancellationToken);

        if (domainEvents.Any() && storedEvents.Any())
        {
            var tasks = domainEvents
                .Select((@event) => _mediator.Publish(@event, cancellationToken));

            // Disparando as notificações.
            await Task.WhenAll(tasks);

            // Salvando os eventos no MongoDB.
            await _eventContext.StoredEvents.InsertManyAsync(storedEvents, cancellationToken: cancellationToken);
        }
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork()
    {
        Dispose(false);
    }

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
            _shopContext.Dispose();

        _disposed = true;
    }

    #endregion
}