using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Data;

public class DbContextTransaction<TContext> : ITransaction<TContext> where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ILogger<TContext> _logger;

    public DbContextTransaction(TContext context, ILogger<TContext> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            _logger.LogInformation("----- Begin transaction {TransactionId}", transaction.TransactionId);

            await operation();

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred while committing the transaction {TransactionId}", transaction.TransactionId);

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        });
    }
}