using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

internal sealed class UnitOfWork(
    WriteDbContext writeDbContext,
    IEventStoreRepository eventStoreRepository,
    IMediator mediator,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    /// <summary>
    /// Saves changes asynchronously.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        // Creating the execution strategy (Connection resiliency and database retries).
        var strategy = writeDbContext.Database.CreateExecutionStrategy();

        // Executing the strategy.
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await writeDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            logger.LogInformation("----- Begin transaction: '{TransactionId}'", transaction.TransactionId);

            try
            {
                // Getting the domain events and event stores from the tracked entities in the EF Core context.
                var (domainEvents, eventStores) = BeforeSaveChanges();

                var rowsAffected = await writeDbContext.SaveChangesAsync();

                logger.LogInformation("----- Commit transaction: '{TransactionId}'", transaction.TransactionId);

                await transaction.CommitAsync();

                // Triggering the events and saving the stores.
                await AfterSaveChangesAsync(domainEvents, eventStores);

                logger.LogInformation(
                    "----- Transaction successfully confirmed: '{TransactionId}', Rows Affected: {RowsAffected}",
                    transaction.TransactionId,
                    rowsAffected);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An unexpected exception occurred while committing the transaction: '{TransactionId}', message: {Message}",
                    transaction.TransactionId,
                    ex.Message);

                await transaction.RollbackAsync();

                throw;
            }
        });
    }

    /// <summary>
    /// Executes logic before saving changes to the database.
    /// </summary>
    /// <returns>A tuple containing the list of domain events and event stores.</returns>
    private (IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore> eventStores) BeforeSaveChanges()
    {
        // Get all domain entities with pending domain events
        var domainEntities = writeDbContext
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        // Get all domain events from the domain entities
        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        // Convert domain events to event stores
        var eventStores = domainEvents
            .ConvertAll(@event => new EventStore(@event.AggregateId, @event.GetGenericTypeName(), @event.ToJson()));

        // Clear domain events from the entities
        domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

        return (domainEvents.AsReadOnly(), eventStores.AsReadOnly());
    }

    /// <summary>
    /// Performs necessary actions after saving changes, such as publishing domain events and storing event stores.
    /// </summary>
    /// <param name="domainEvents">The list of domain events.</param>
    /// <param name="eventStores">The list of event stores.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AfterSaveChangesAsync(
        IReadOnlyList<BaseEvent> domainEvents,
        IReadOnlyList<EventStore> eventStores)
    {
        if (!domainEvents.Any())
            return;

        if (!eventStores.Any())
            return;

        // Publish each domain event using _mediator.
        await Task.WhenAll(domainEvents.Select(@event => mediator.Publish(@event)));

        // Store the event stores using _eventStoreRepository.
        await eventStoreRepository.StoreAsync(eventStores);
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork() => Dispose(false);

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
            writeDbContext.Dispose();
            eventStoreRepository.Dispose();
        }

        _disposed = true;
    }

    #endregion
}