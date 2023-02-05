using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.Customer;
using Shop.Domain.Interfaces;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ShopContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().AnyAsync(customer => customer.Email.Address == email.Address, cancellationToken);
}