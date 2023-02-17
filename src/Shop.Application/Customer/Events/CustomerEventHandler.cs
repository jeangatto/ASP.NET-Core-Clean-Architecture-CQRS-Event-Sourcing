using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Shop.Core.Interfaces;
using Shop.Domain.Entities.Customer.Events;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Events;

/// <summary>
/// Manipulador de Eventos do Cliente.
/// </summary>
public class CustomerEventHandler :
    INotificationHandler<CustomerCreatedEvent>,
    INotificationHandler<CustomerUpdatedEvent>,
    INotificationHandler<CustomerDeletedEvent>
{
    private readonly IMapper _mapper;
    private readonly ISyncDataBase _syncDataBase;

    public CustomerEventHandler(IMapper mapper, ISyncDataBase syncDataBase)
    {
        _mapper = mapper;
        _syncDataBase = syncDataBase;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
        => await SaveAsync(_mapper.Map<CustomerQueryModel>(notification));

    public async Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
        => await SaveAsync(_mapper.Map<CustomerQueryModel>(notification));

    public async Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
        => await _syncDataBase.DeleteAsync<CustomerQueryModel>(filter => filter.Id == notification.Id);

    private async Task SaveAsync(CustomerQueryModel queryModel)
        => await _syncDataBase.SaveAsync(queryModel, filter => filter.Id == queryModel.Id);
}