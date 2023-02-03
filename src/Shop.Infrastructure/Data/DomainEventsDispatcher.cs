using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shop.Core.Abstractions;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly ShopContext _context;
    private readonly IMediator _mediator;

    public DomainEventsDispatcher(ShopContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        if (domainEntities.Any())
        {
            var domainEvents = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            domainEntities
                .ForEach(entry => entry.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select((@event) => _mediator.Publish(@event, cancellationToken));

            await Task.WhenAll(tasks);
        }
    }
}