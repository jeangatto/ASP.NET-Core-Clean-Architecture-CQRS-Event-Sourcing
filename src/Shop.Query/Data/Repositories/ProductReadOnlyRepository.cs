using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class ProductReadOnlyRepository(IReadDbContext readDbContext) : BaseReadOnlyRepository<ProductQueryModel, Guid>(readDbContext), IProductReadOnlyRepository
{
    public async Task<IEnumerable<ProductQueryModel>> GetAllAsync() =>
        await Collection
            .Find(Builders<ProductQueryModel>.Filter.Empty)
            .SortBy(product => product.Name)
            .ThenBy(product => product.Description)
            .ToListAsync();
}