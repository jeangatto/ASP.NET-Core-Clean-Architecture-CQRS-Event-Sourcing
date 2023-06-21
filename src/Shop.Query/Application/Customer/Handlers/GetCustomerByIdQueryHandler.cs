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

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
{
    private readonly ICacheService _cacheService;
    private readonly ICustomerReadOnlyRepository _repository;
    private readonly IValidator<GetCustomerByIdQuery> _validator;

    public GetCustomerByIdQueryHandler(
        IValidator<GetCustomerByIdQuery> validator,
        ICustomerReadOnlyRepository repository,
        ICacheService cacheService)
    {
        _validator = validator;
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<CustomerQueryModel>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Retorna o resultado com os erros da validação.
            return Result<CustomerQueryModel>.Invalid(validationResult.AsErrors());
        }

        var cacheKey = $"{nameof(GetCustomerByIdQuery)}_{request.Id}";

        // Obtendo o cliente do base e inserindo no serviço de cache
        // Na próxima consulta irá buscar no serviço de cache
        var customer = await _cacheService.GetOrCreateAsync(cacheKey, () => _repository.GetByIdAsync(request.Id));
        return customer == null
            ? Result<CustomerQueryModel>.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}")
            : Result<CustomerQueryModel>.Success(customer);
    }
}