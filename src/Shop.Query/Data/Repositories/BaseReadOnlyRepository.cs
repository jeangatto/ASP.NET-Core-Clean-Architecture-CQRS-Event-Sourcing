using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;
using Shop.Query.Data.Context;

namespace Shop.Query.Data.Repositories;

public abstract class BaseReadOnlyRepository<T> : IReadOnlyRepository<T, Guid> where T : class, IQueryModel<Guid>
{
    protected readonly IMongoCollection<T> Collection;

    protected BaseReadOnlyRepository(ReadDbContext readDbContext)
        => Collection = readDbContext.GetCollection<T>();

    public async Task<T> GetByIdAsync(Guid id)
        => await Collection
            .Find(queryModel => queryModel.Id == id)
            .FirstOrDefaultAsync();
}