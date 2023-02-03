using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken = default);
}