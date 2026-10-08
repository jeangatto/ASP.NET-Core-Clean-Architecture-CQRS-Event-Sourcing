using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data.Context;
using Xunit;

namespace Shop.UnitTests.Fixtures;

public class EfSqliteFixture : IAsyncLifetime
{
    private const string ConnectionString = "Data Source=:memory:";
    private readonly SqliteConnection _connection;

    public EfSqliteFixture()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        var builder = new DbContextOptionsBuilder<WriteDbContext>().UseSqlite(_connection);
        DbContext = new WriteDbContext(builder.Options);
    }

    public WriteDbContext DbContext { get; }

    #region IAsyncLifetime

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #endregion
}