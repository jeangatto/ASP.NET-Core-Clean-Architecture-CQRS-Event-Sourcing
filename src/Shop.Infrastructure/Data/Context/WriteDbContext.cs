using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class WriteDbContext(DbContextOptions<WriteDbContext> dbOptions)
    : BaseDbContext<WriteDbContext>(dbOptions)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}