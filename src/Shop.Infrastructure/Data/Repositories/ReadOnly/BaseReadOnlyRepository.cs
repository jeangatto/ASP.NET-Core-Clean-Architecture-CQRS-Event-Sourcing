using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Core.Abstractions;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.ReadOnly;

public abstract class BaseReadOnlyRepository<TQueryModel> : IReadOnlyRepository<TQueryModel>
    where TQueryModel : BaseQueryModel
{
    protected readonly IMongoCollection<TQueryModel> Collection;

    protected BaseReadOnlyRepository(ReadDbContext readDbContext)
        => Collection = readDbContext.GetCollection<TQueryModel>();

    public async Task<TQueryModel> GetByIdAsync(Guid id)
        => await Collection.Find(query => query.Id == id).FirstOrDefaultAsync();
}