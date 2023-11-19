using Microsoft.EntityFrameworkCore;
using Shop.Core.SharedKernel;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions) : BaseDbContext<EventStoreDbContext>(dbOptions)
{
    public DbSet<EventStore> EventStores => Set<EventStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}