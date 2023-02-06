using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shop.Domain.Entities.Customer.Events;

namespace Shop.Application.Customer.Events;

public class CustomerEventHandler :
    INotificationHandler<CustomerCreatedEvent>,
    INotificationHandler<CustomerUpdatedEvent>
{
    public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}