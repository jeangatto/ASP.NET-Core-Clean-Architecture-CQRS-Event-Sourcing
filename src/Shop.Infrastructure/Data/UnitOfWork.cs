using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopContext _context;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ShopContext context, IDomainEventsDispatcher domainEventsDispatcher, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _domainEventsDispatcher = domainEventsDispatcher;
        _logger = logger;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _domainEventsDispatcher.DispatchAsync(cancellationToken);

            var rowsAffected = await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("----- Row(s) affected: {RowsAffected}", rowsAffected);

            return rowsAffected;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Ocorreu um erro (concorrência) ao salvar as informações na base de dados");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao salvar as informações na base de dados");
            throw;
        }
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~UnitOfWork()
    {
        Dispose(false);
    }

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // Dispose managed state (managed objects).
        if (disposing)
            _context.Dispose();

        _disposed = true;
    }

    #endregion
}