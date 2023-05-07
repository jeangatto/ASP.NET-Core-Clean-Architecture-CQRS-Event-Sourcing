using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;

namespace Shop.Query.Data.Repositories;

internal abstract class BaseReadOnlyRepository<TQueryModel> : IReadOnlyRepository<TQueryModel>
    where TQueryModel : IQueryModel<Guid>
{
    protected readonly IMongoCollection<TQueryModel> Collection;

    protected BaseReadOnlyRepository(IReadDbContext context)
        => Collection = context.GetCollection<TQueryModel>();

    public async Task<TQueryModel> GetByIdAsync(Guid id)
        => await Collection
            .Find(queryModel => queryModel.Id == id)
            .FirstOrDefaultAsync();
}