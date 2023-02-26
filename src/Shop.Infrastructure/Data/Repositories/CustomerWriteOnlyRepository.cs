using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Entities.CustomerAggregate.Repositories;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public class CustomerWriteOnlyRepository : BaseWriteOnlyRepository<Customer>, ICustomerWriteOnlyRepository
{
    public CustomerWriteOnlyRepository(WriteDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
        => await DbSet
            .AsNoTracking()
            .AnyAsync(customer => customer.Email.Address == email.Address);

    public async Task<bool> ExistsByEmailAsync(Email email, Guid currentId)
        => await DbSet
            .AsNoTracking()
            .AnyAsync(customer => customer.Email.Address == email.Address && customer.Id != currentId);
}