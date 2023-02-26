using Microsoft.EntityFrameworkCore;
using Shop.Core.Events;
using Shop.Infrastructure.Data.Extensions;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class EventStoreDbContext : DbContext
{
    private const string Collation = "Latin1_General_CI_AI";

    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<EventStore> EventStores => Set<EventStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation(Collation);
        modelBuilder.RemoveCascadeDeleteConvention();
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}