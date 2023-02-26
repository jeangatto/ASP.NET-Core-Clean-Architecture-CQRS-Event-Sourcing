using System;
using System.Threading.Tasks;
using Shop.Core.Interfaces;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Entities.CustomerAggregate.Repositories;

public interface ICustomerWriteOnlyRepository : IWriteOnlyRepository<Customer, Guid>
{
    Task<bool> ExistsByEmailAsync(Email email);
    Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
}