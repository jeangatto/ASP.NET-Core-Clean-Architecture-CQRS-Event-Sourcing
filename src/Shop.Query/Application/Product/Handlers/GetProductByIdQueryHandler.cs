using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Core.SharedKernel;
using Shop.Query.Application.Product.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Product.Handlers;

public class GetProductByIdQueryHandler(
    IValidator<GetProductByIdQuery> validator,
    IProductReadOnlyRepository repository,
    ICacheService cacheService) : IRequestHandler<GetProductByIdQuery, Result<ProductQueryModel>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IProductReadOnlyRepository _repository = repository;
    private readonly IValidator<GetProductByIdQuery> _validator = validator;

    public async Task<Result<ProductQueryModel>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result<ProductQueryModel>.Invalid(validationResult.AsErrors());
        }

        // Creating a cache key using the query name and the Product ID.
        var cacheKey = $"{nameof(GetProductByIdQuery)}_{request.Id}";

        // Getting the Product from the cache service. If not found, fetches it from the repository.
        // The Product will be stored in the cache service for future queries.
        var product = await _cacheService.GetOrCreateAsync(cacheKey, () => _repository.GetByIdAsync(request.Id));

        // If the Product is null, returns a result indicating that no Product was found.
        // Otherwise, returns a successful result with the Product.
        return product == null
            ? Result<ProductQueryModel>.NotFound($"No Product found by Id: {request.Id}")
            : Result<ProductQueryModel>.Success(product);
    }
}