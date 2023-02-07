using System.Threading.Tasks;
using Shop.Core.Interfaces;
using Shop.Domain.Entities.Customer;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Interfaces.WriteOnly;

public interface ICustomerWriteOnlyRepository : IWriteOnlyRepository<Customer>
{
    Task<bool> ExistsByEmailAsync(Email email);
}