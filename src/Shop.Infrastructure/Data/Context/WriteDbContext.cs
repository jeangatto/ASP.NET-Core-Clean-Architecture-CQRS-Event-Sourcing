using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Extensions;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class WriteDbContext : DbContext
{
    private const string Collation = "Latin1_General_CI_AI";

    public WriteDbContext(DbContextOptions<WriteDbContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation(Collation);
        modelBuilder.RemoveCascadeDeleteConvention();
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    }
}