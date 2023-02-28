using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Shop.Core.Abstractions;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Handlers;

public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>
{
    private readonly ICustomerReadOnlyRepository _readOnlyRepository;
    private readonly ICacheService _cacheService;

    public GetAllCustomerQueryHandler(ICustomerReadOnlyRepository repository, ICacheService cacheService)
    {
        _readOnlyRepository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = nameof(GetAllCustomerQuery);
        return Result.Success(await _cacheService.GetOrCreateAsync(cacheKey, _readOnlyRepository.GetAllAsync));
    }
}