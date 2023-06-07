using System;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Entities.CustomerAggregate;

public interface ICustomerWriteOnlyRepository : IWriteOnlyRepository<Customer>
{
    Task<bool> ExistsByEmailAsync(Email email);
    Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
}