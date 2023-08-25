using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Context;

public abstract class BaseDbContext<TContext> : DbContext
    where TContext : DbContext
{
    private const string Collation = "Latin1_General_CI_AI";

    protected BaseDbContext(DbContextOptions<TContext> dbOptions) : base(dbOptions)
    {
    }

    public override ChangeTracker ChangeTracker
    {
        get
        {
            base.ChangeTracker.LazyLoadingEnabled = false;
            base.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
            base.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
            return base.ChangeTracker;
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<string>().AreUnicode(false).HaveMaxLength(255);

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.UseCollation(Collation).RemoveCascadeDeleteConvention();
}