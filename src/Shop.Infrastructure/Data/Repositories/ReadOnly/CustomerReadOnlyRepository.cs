using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Domain.Interfaces.ReadOnly;
using Shop.Domain.QueriesModel;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories.ReadOnly;

public class CustomerReadOnlyRepository : BaseReadOnlyRepository<CustomerQueryModel>, ICustomerReadOnlyRepository
{
    public CustomerReadOnlyRepository(ReadDbContext readDbContext) : base(readDbContext)
    {
    }

    public async Task<CustomerQueryModel> GetByEmailAsync(string email)
        => await Collection.Find(query => query.Email == email).FirstOrDefaultAsync();

    public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
    {
        var filter = Builders<CustomerQueryModel>.Filter.Empty;
        return await Collection
            .Find(filter)
            .SortBy(customer => customer.FirstName)
            .ThenBy(customer => customer.DateOfBirth)
            .ToListAsync();
    }
}