using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.Core.Shared;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.Abstractions;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.QueriesModel;

namespace Shop.Query.EventHandlers;

/// <summary>
/// Manipulador de Eventos do Cliente.
/// </summary>
public class CustomerEventHandler :
    INotificationHandler<CustomerCreatedEvent>,
    INotificationHandler<CustomerUpdatedEvent>,
    INotificationHandler<CustomerDeletedEvent>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CustomerEventHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IReadDbContext _readDbContext;

    public CustomerEventHandler(
        IMapper mapper,
        IReadDbContext readDbContext,
        ICacheService cacheService,
        ILogger<CustomerEventHandler> logger)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
        await _readDbContext.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        await _readDbContext.DeleteAsync<CustomerQueryModel>(filter => filter.Email == notification.Email);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
        await _readDbContext.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    private async Task ClearCacheAsync(CustomerBaseEvent @event)
    {
        var cacheKeys = new[] { nameof(GetAllCustomerQuery), $"{nameof(GetCustomerByIdQuery)}_{@event.Id}" };
        await _cacheService.RemoveAsync(cacheKeys);
    }

    private void LogEvent<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent
        => _logger.LogInformation(
            "----- Triggering the event {EventName}, model: {EventModel}", typeof(TEvent).Name, @event.ToJson());
}