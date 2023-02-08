using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class NoSqlSyncDataBase : ISyncDataBase
{
    private readonly ReadDbContext _readDbContext;

    public NoSqlSyncDataBase(ReadDbContext readDbContext)
        => _readDbContext = readDbContext;

    public async Task SaveAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel
    {
        var collection = _readDbContext.GetCollection<TQueryModel>();

        // Se o documento existir, será substituído.
        // Se o documento não existir, será criado um novo.
        await collection.ReplaceOneAsync(upsertFilter, queryModel, new ReplaceOptions { IsUpsert = true });
    }
}