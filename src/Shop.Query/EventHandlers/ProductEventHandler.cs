using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.ProductAggregate.Events;
using Shop.Query.Abstractions;
using Shop.Query.Application.Product.Queries;
using Shop.Query.QueriesModel;

namespace Shop.Query.EventHandlers;

public class ProductEventHandler(
    IMapper mapper,
    ISynchronizeDb synchronizeDb,
    ICacheService cacheService,
    ILogger<ProductEventHandler> logger) :
    INotificationHandler<ProductCreatedEvent>,
    INotificationHandler<ProductUpdatedEvent>,
    INotificationHandler<ProductDeletedEvent>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<ProductEventHandler> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ISynchronizeDb _synchronizeDb = synchronizeDb;

    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var productQueryModel = _mapper.Map<ProductQueryModel>(notification);
        await _synchronizeDb.UpsertAsync(productQueryModel, filter => filter.Id == productQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        await _synchronizeDb.DeleteAsync<ProductQueryModel>(filter => filter.Id == notification.Id);
        await ClearCacheAsync(notification);
    }

    public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var productQueryModel = _mapper.Map<ProductQueryModel>(notification);
        await _synchronizeDb.UpsertAsync(productQueryModel, filter => filter.Id == productQueryModel.Id);
        await ClearCacheAsync(notification);
    }

    private async Task ClearCacheAsync(ProductBaseEvent @event)
    {
        var cacheKeys = new[] { nameof(GetAllProductQuery), $"{nameof(GetProductByIdQuery)}_{@event.Id}" };
        await _cacheService.RemoveAsync(cacheKeys);
    }

    private void LogEvent<TEvent>(TEvent @event) where TEvent : ProductBaseEvent =>
        _logger.LogInformation("----- Triggering the event {EventName}, model: {EventModel}", typeof(TEvent).Name, @event.ToJson());
}
