using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.Customer;
using Shop.Domain.Interfaces.WriteOnly;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.WriteOnly;

public class CustomerWriteOnlyRepository : BaseWriteOnlyRepository<Customer>, ICustomerWriteOnlyRepository
{
    public CustomerWriteOnlyRepository(WriteDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
        => await DbSet.AsNoTracking().AnyAsync(customer => customer.Email.Address == email.Address);
}