using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.Customer;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ShopContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByEmailAsync(string email)
        => await DbSet.AsNoTracking().AnyAsync(customer => customer.Email == email);
}