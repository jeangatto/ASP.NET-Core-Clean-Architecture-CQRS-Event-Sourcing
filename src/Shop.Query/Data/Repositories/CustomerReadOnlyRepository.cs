using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class CustomerReadOnlyRepository(IReadDbContext readDbContext)
    : BaseReadOnlyRepository<CustomerQueryModel, Guid>(readDbContext), ICustomerReadOnlyRepository
{
    public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync() =>
        await Collection
            .Find(Builders<CustomerQueryModel>.Filter.Empty)
            .SortBy(customer => customer.FirstName)
            .ThenBy(customer => customer.DateOfBirth)
            .ToListAsync();
}