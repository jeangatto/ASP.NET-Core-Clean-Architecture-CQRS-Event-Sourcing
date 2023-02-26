using Microsoft.EntityFrameworkCore;
using Shop.Core.Events;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class EventStoreDbContext : BaseDbContext<EventStoreDbContext>
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<EventStore> EventStores => Set<EventStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}