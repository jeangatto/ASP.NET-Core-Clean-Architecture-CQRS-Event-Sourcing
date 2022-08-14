using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Context;

public class ShopContext : DbContext
{
    private readonly string _collation;
    private readonly ICurrentUserProvider _currentUserProvider;

    public ShopContext(DbContextOptions<ShopContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public ShopContext(
        DbContextOptions<ShopContext> dbOptions,
        ICurrentUserProvider currentUserProvider) : this(dbOptions)
    {
        _currentUserProvider = currentUserProvider;
    }

    public ShopContext(
        DbContextOptions<ShopContext> dbOptions,
        ICurrentUserProvider currentUserProvider,
        IOptions<ConnectionOptions> connectionOptions) : this(dbOptions, currentUserProvider)
    {
        _collation = connectionOptions.Value.Collation;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(_collation))
            modelBuilder.UseCollation(_collation);

        modelBuilder
                .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                .RemoveCascadeDeleteConvention();
    }

    public override int SaveChanges()
    {
        OnBeforeSaving();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        foreach (var entry in ChangeTracker.Entries<IAudit>())
        {
            var userId = _currentUserProvider?.GetCurrentUserId();

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.Version++;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = DateTime.Now;
                    entry.Entity.Version++;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = DateTime.Now;
                    entry.Entity.IsDeleted = true;
                    break;
            }
        }
    }
}