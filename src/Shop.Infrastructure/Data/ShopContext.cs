using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shop.Infrastructure.Data;

public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    #region Transaction

    private IDbContextTransaction _currentTransaction;

<<<<<<< HEAD
    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => _currentTransaction ??= await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _currentTransaction?.CommitAsync();
=======
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _currentTransaction ??= await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken);
>>>>>>> c96991cfd332ad3865ee7ed214a6e0014c7ca12f
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

<<<<<<< HEAD
    private async Task RollbackTransactionAsync()
=======
    private async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
>>>>>>> c96991cfd332ad3865ee7ed214a6e0014c7ca12f
    {
        try
        {
            await _currentTransaction?.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    #endregion
}