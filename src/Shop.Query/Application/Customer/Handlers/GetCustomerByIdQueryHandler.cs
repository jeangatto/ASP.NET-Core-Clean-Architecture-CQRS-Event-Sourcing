using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Core.Abstractions;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Handlers;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
{
    private readonly GetCustomerByIdQueryValidator _validator;
    private readonly ICustomerReadOnlyRepository _repository;
    private readonly ICacheService _cacheService;

    public GetCustomerByIdQueryHandler(
        GetCustomerByIdQueryValidator validator,
        ICustomerReadOnlyRepository repository,
        ICacheService cacheService)
    {
        _validator = validator;
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<CustomerQueryModel>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        var cacheKey = $"{nameof(GetCustomerByIdQuery)}_{request.Id}";

        // Obtendo o cliente da base.
        var customer = await _cacheService.GetOrCreateAsync(cacheKey, () => _repository.GetByIdAsync(request.Id));
        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Retornando o cliente no resultado.
        return Result.Success(customer);
    }
}