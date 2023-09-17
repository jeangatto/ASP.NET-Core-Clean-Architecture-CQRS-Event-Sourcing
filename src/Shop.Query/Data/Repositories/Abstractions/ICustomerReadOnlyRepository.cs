using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories.Abstractions;

public interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel, Guid>
{
    Task<IEnumerable<CustomerQueryModel>> GetAllAsync();
}
