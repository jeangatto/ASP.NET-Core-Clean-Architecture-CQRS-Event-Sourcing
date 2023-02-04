using System.Threading.Tasks;
using Shop.Core.Interfaces;
using Shop.Domain.Entities.Customer;

namespace Shop.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetByEmailAsync(string email);
}