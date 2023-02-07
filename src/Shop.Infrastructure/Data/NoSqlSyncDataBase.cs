using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class NoSqlSyncDataBase : ISyncDataBase
{
    private readonly ReadDbContext _readDbContext;

    public NoSqlSyncDataBase(ReadDbContext readDbContext) => _readDbContext = readDbContext;

    public async Task SaveAsync<TQueryModel>(TQueryModel queryModel) where TQueryModel : IQueryModel
    {
        var collection = _readDbContext.GetCollection<TQueryModel>();
        await collection.InsertOneAsync(queryModel, new InsertOneOptions());
    }
}