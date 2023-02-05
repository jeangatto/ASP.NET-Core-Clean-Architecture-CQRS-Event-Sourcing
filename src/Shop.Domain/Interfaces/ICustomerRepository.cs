using System.Threading;
using System.Threading.Tasks;
using Shop.Core.Interfaces;
using Shop.Domain.Entities.Customer;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}