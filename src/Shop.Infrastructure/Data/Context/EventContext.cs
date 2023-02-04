using Microsoft.EntityFrameworkCore;
using Shop.Core.Events;
using Shop.Infrastructure.Data.Configurations;

namespace Shop.Infrastructure.Data.Context;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StoredEventConfiguration());
    }
}