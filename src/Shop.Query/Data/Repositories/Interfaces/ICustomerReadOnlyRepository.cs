using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories.Interfaces;

public interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel>
{
    Task<CustomerQueryModel> GetByEmailAsync(string email);
    Task<IEnumerable<CustomerQueryModel>> GetAllAsync();
}