using System;
using System.Threading.Tasks;
using Shop.Core.Abstractions;

namespace Shop.Core.Interfaces;

public interface IReadOnlyRepository<TQueryModel> where TQueryModel : BaseQueryModel
{
    Task<TQueryModel> GetByIdAsync(Guid id);
}