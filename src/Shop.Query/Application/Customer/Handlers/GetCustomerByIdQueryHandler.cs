using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Core.SharedKernel;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Handlers;

public class GetCustomerByIdQueryHandler(
    IValidator<GetCustomerByIdQuery> validator,
    ICustomerReadOnlyRepository repository,
    ICacheService cacheService) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
{
    public async Task<Result<CustomerQueryModel>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result<CustomerQueryModel>.Invalid(validationResult.AsErrors());
        }

        // Creating a cache key using the query name and the customer ID.
        var cacheKey = $"{nameof(GetCustomerByIdQuery)}_{request.Id}";

        // Getting the customer from the cache service. If not found, fetches it from the repository.
        // The customer will be stored in the cache service for future queries.
        var customer = await cacheService.GetOrCreateAsync(cacheKey, () => repository.GetByIdAsync(request.Id));

        // If the customer is null, returns a result indicating that no customer was found.
        // Otherwise, returns a successful result with the customer.
        return customer == null
            ? Result<CustomerQueryModel>.NotFound($"No customer found by Id: {request.Id}")
            : Result<CustomerQueryModel>.Success(customer);
    }
}