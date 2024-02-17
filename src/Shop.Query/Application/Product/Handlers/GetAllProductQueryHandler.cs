using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Shop.Core.SharedKernel;
using Shop.Query.Application.Product.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Product.Handlers;

public class GetAllProductQueryHandler(IProductReadOnlyRepository repository, ICacheService cacheService) : IRequestHandler<GetAllProductQuery, Result<IEnumerable<ProductQueryModel>>>
{
    private const string CacheKey = nameof(GetAllProductQuery);
    private readonly ICacheService _cacheService = cacheService;
    private readonly IProductReadOnlyRepository _readOnlyRepository = repository;

    public async Task<Result<IEnumerable<ProductQueryModel>>> Handle(
          GetAllProductQuery request,
          CancellationToken cancellationToken)
    {
        // This method will either return the cached data associated with the CacheKey
        // or create it by calling the GetAllAsync method.
        return Result<IEnumerable<ProductQueryModel>>.Success(
            await _cacheService.GetOrCreateAsync(CacheKey, _readOnlyRepository.GetAllAsync));
    }
}