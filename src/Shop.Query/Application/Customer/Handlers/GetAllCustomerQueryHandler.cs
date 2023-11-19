using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Shop.Core.SharedKernel;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Handlers;

public class GetAllCustomerQueryHandler(ICustomerReadOnlyRepository repository, ICacheService cacheService) : IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>
{
    private const string CacheKey = nameof(GetAllCustomerQuery);
    private readonly ICacheService _cacheService = cacheService;
    private readonly ICustomerReadOnlyRepository _readOnlyRepository = repository;

    public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(
          GetAllCustomerQuery request,
          CancellationToken cancellationToken)
    {
        // This method will either return the cached data associated with the CacheKey
        // or create it by calling the GetAllAsync method.
        return Result<IEnumerable<CustomerQueryModel>>.Success(
            await _cacheService.GetOrCreateAsync(CacheKey, _readOnlyRepository.GetAllAsync));
    }
}