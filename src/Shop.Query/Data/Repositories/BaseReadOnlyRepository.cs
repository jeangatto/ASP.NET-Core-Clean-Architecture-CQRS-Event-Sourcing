using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;

namespace Shop.Query.Data.Repositories;

internal abstract class BaseReadOnlyRepository<TQueryModel> : IReadOnlyRepository<TQueryModel>
    where TQueryModel : IQueryModel<Guid>
{
    private readonly IReadDbContext _context;
    protected readonly IMongoCollection<TQueryModel> Collection;

    protected BaseReadOnlyRepository(IReadDbContext context)
    {
        _context = context;
        Collection = context.GetCollection<TQueryModel>();
    }

    public async Task<TQueryModel> GetByIdAsync(Guid id)
        => await Collection
            .Find(queryModel => queryModel.Id == id)
            .FirstOrDefaultAsync();

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~BaseReadOnlyRepository()
        => Dispose(false);

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