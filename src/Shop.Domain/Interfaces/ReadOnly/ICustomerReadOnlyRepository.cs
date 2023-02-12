using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Core.Interfaces;
using Shop.Domain.QueriesModel;

namespace Shop.Domain.Interfaces.ReadOnly;

public interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel>
{
    Task<CustomerQueryModel> GetByEmailAsync(string email);
    Task<IEnumerable<CustomerQueryModel>> GetAllAsync();
}