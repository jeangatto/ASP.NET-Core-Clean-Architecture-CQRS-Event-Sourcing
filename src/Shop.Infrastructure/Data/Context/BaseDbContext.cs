using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Context;

public abstract class BaseDbContext<TContext> : DbContext
    where TContext : DbContext
{
    private const string Collation = "Latin1_General_CI_AI";

    protected BaseDbContext(DbContextOptions<TContext> dbOptions) : base(dbOptions)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .UseCollation(Collation)
            .RemoveCascadeDeleteConvention();
}