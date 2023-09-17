using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data.Context;
using Xunit;

namespace Shop.UnitTests.Fixtures;

public class EfSqliteFixture : IAsyncLifetime, IDisposable
{
    private const string ConnectionString = "Data Source=:memory:";
    private readonly SqliteConnection _connection;

    public EfSqliteFixture()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        var builder = new DbContextOptionsBuilder<WriteDbContext>().UseSqlite(_connection);
        Context = new WriteDbContext(builder.Options);
    }

    public WriteDbContext Context { get; }

    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #endregion

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    ~EfSqliteFixture() => Dispose(false);

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
        {
            _connection?.Dispose();
            Context?.Dispose();
        }

        _disposed = true;
    }

    #endregion
}
