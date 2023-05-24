using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Domain;
using Shop.Core.Events;
using Shop.Core.Extensions;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IMediator _mediator;
    private readonly WriteDbContext _writeDbContext;

    public UnitOfWork(
        WriteDbContext writeDbContext,
        IEventStoreRepository eventStoreRepository,
        IMediator mediator,
        ILogger<UnitOfWork> logger)
    {
        _writeDbContext = writeDbContext;
        _eventStoreRepository = eventStoreRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task SaveChangesAsync()
    {
        // Criando a estratégia de execução (Connection resiliency and database retries).
        var strategy = _writeDbContext.Database.CreateExecutionStrategy();

        // Executando a estratégia.
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction
                = await _writeDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            _logger.LogInformation("----- Begin transaction: '{TransactionId}'", transaction.TransactionId);

            try
            {
                // Obtendo os eventos e stores das entidades rastreadas no contexto do EF Core.
                var (domainEvents, eventStores) = BeforeSaveChanges();

                var rowsAffected = await _writeDbContext.SaveChangesAsync();

                _logger.LogInformation("----- Commit transaction: '{TransactionId}'", transaction.TransactionId);

                await transaction.CommitAsync();

                // Disparando os eventos e salvando os stores.
                await AfterSaveChangesAsync(domainEvents, eventStores);

                _logger.LogInformation(
                    "----- Transaction successfully confirmed: '{TransactionId}', Rows Affected: {RowsAffected}",
                    transaction.TransactionId, rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An unexpected exception occurred while committing the transaction: '{TransactionId}', message: {Message}",
                    transaction.TransactionId, ex.Message);

                await transaction.RollbackAsync();

                throw;
            }
        });
    }

    private (IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore> eventStores) BeforeSaveChanges()
    {
        var domainEntities = _writeDbContext
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        var eventStores = domainEvents
            .ConvertAll(@event => new EventStore(@event.AggregateId, @event.GetGenericTypeName(), @event.ToJson()));

        domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

        return (domainEvents.AsReadOnly(), eventStores.AsReadOnly());
    }

    private async Task AfterSaveChangesAsync(
        IReadOnlyList<BaseEvent> domainEvents,
        IReadOnlyList<EventStore> eventStores)
    {
        if (!domainEvents.Any() || !eventStores.Any())
            return;

        var tasks = domainEvents
            .AsParallel()
            .Select(@event => _mediator.Publish(@event))
            .ToList();

        await Task.WhenAll(tasks);

        await _eventStoreRepository.StoreAsync(eventStores);
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork()
        => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
        {
            _writeDbContext.Dispose();
            _eventStoreRepository.Dispose();
        }

        _disposed = true;
    }

    #endregion
}