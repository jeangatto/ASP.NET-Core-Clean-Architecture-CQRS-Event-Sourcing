using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.Customer;
using Shop.Infrastructure.Data.Extensions;

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
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.RemoveCascadeDeleteConvention();
    }
}