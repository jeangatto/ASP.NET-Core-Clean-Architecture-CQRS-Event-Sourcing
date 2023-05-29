using Microsoft.EntityFrameworkCore;
using Shop.Core.Shared;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class EventStoreDbContext : BaseDbContext<EventStoreDbContext>
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions) : base(dbOptions)
    {
    }

    internal DbSet<EventStore> EventStores => Set<EventStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}