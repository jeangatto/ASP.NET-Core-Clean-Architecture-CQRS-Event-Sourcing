using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class WriteDbContext : BaseDbContext<WriteDbContext>
{
    public WriteDbContext(DbContextOptions<WriteDbContext> dbOptions) : base(dbOptions)
    {
    }

    internal DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}