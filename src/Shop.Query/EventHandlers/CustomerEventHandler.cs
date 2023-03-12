using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Shop.Core.Abstractions;
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
    private readonly IMapper _mapper;
    private readonly IReadDbContext _readDbContext;
    private readonly ICacheService _cacheService;

    public CustomerEventHandler(
        IMapper mapper,
        IReadDbContext readDbContext,
        ICacheService cacheService)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
        _cacheService = cacheService;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        var customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
        await _readDbContext.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
        await _readDbContext.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _readDbContext.DeleteAsync<CustomerQueryModel>(filter => filter.Id == notification.Id);
        await ClearCacheAsync(notification);
    }

    private async Task ClearCacheAsync(CustomerBaseEvent @event)
    {
        var cacheKeys = new[] { nameof(GetAllCustomerQuery), $"{nameof(GetCustomerByIdQuery)}_{@event.Id}" };
        await _cacheService.RemoveAsync(cacheKeys);
    }
}