using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Abstractions;
using Shop.Core.Events;
using Shop.Core.Extensions;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    #region Constructor

    private readonly ShopContext _shopContext;
    private readonly EventContext _eventContext;
    private readonly IMediator _mediator;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        ShopContext shopContext,
        EventContext eventContext,
        IMediator mediator,
        ILogger<UnitOfWork> logger)
    {
        _shopContext = shopContext;
        _eventContext = eventContext;
        _mediator = mediator;
        _logger = logger;
    }

    #endregion

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await PublishDomainEvents(cancellationToken);

            var rowsAffected = await _shopContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("----- Row(s) affected: {RowsAffected}", rowsAffected);

            return rowsAffected;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Ocorreu um erro (concorrência) ao salvar as informações na base de dados");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao salvar as informações na base de dados");
            throw;
        }
    }

    private async Task PublishDomainEvents(CancellationToken cancellationToken = default)
    {
        var domainEntities = _shopContext.ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        if (domainEntities.Any())
        {
            var domainEvents = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            var storedEvents = new List<StoredEvent>();

            foreach (var @event in domainEvents)
            {
                var type = @event.GetGenericTypeName();
                var data = @event.ToJson();
                storedEvents.Add(new StoredEvent(type, data));
            }

            domainEntities
                .ForEach(entry => entry.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select((@event) => _mediator.Publish(@event, cancellationToken));

            await Task.WhenAll(tasks);

            _eventContext.StoredEvents.AddRange(storedEvents);
            await _eventContext.SaveChangesAsync(cancellationToken);
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
        {
            _shopContext.Dispose();
            _eventContext.Dispose();
        }

        _disposed = true;
    }

    #endregion
}