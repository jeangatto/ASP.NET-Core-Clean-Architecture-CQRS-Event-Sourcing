using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Context;

public class ShopContext : DbContext
{
    private readonly string _collation;

    public ShopContext(DbContextOptions<ShopContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public ShopContext(IOptions<ConnectionOptions> options, DbContextOptions<ShopContext> dbOptions) : this(dbOptions)
    {
        _collation = options.Value.Collation;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(_collation))
            modelBuilder.UseCollation(_collation);

        modelBuilder
                .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                .RemoveCascadeDeleteConvention();
    }
}