using System;
using System.Threading.Tasks;
using Shop.Core.Domain;
using Shop.Core.ValueObjects;

namespace Shop.Domain.Entities.CustomerAggregate;

public interface ICustomerWriteOnlyRepository : IWriteOnlyRepository<Customer>
{
    Task<bool> ExistsByEmailAsync(Email email);
    Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
}