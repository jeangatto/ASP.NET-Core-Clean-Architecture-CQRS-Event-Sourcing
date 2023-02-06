using System;
using System.Collections.Generic;
using System.Data;
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
using Shop.Infrastructure.Data.Events;

namespace Shop.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
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

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
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

            var rowsAffected = await _shopContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("----- Row(s) affected: {RowsAffected}", rowsAffected);

            if (domainEvents.Any() && storedEvents.Any())
            {
                var tasks = domainEvents
                    .Select((@event) => _mediator.Publish(@event, cancellationToken));

                // Disparando as notificações.
                await Task.WhenAll(tasks);

                // Salvando os eventos no MongoDB.
                await _eventContext.StoredEvents.InsertManyAsync(storedEvents, cancellationToken: cancellationToken);
            }

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
}