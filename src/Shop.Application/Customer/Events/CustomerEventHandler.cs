using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Shop.Core.Interfaces;
using Shop.Domain.Entities.Customer.Events;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Events;

public class CustomerEventHandler : INotificationHandler<CustomerCreatedEvent>
{
    private readonly IMapper _mapper;
    private readonly ISyncDataBase _syncDataBase;

    public CustomerEventHandler(IMapper mapper, ISyncDataBase syncDataBase)
    {
        _mapper = mapper;
        _syncDataBase = syncDataBase;
    }

    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
        => await _syncDataBase.SaveAsync(_mapper.Map<CustomerQueryModel>(notification));
}