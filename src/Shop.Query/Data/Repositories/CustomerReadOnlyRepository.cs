using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

public class CustomerReadOnlyRepository : BaseReadOnlyRepository<CustomerQueryModel>, ICustomerReadOnlyRepository
{
    public CustomerReadOnlyRepository(IReadDbContext readDbContext) : base(readDbContext) { }

    public async Task<CustomerQueryModel> GetByEmailAsync(string email)
        => await Collection
            .Find(customer => customer.Email == email)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
        => await Collection
            .Find(Builders<CustomerQueryModel>.Filter.Empty)
            .SortBy(customer => customer.FirstName)
            .ThenBy(customer => customer.DateOfBirth)
            .ToListAsync();
}