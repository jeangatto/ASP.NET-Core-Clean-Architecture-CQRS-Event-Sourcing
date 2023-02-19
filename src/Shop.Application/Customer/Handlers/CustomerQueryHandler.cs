using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Customer.Queries;
using Shop.Application.Customer.Validators;
using Shop.Core.Interfaces;
using Shop.Domain.Interfaces.ReadOnly;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Handlers;

public class CustomerQueryHandler :
    IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>,
    IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
{
    private readonly GetCustomerByIdQueryValidator _validator;
    private readonly ICustomerReadOnlyRepository _readOnlyRepository;
    private readonly ICacheService _cache;

    public CustomerQueryHandler(
        GetCustomerByIdQueryValidator validator,
        ICustomerReadOnlyRepository readOnlyRepository,
        ICacheService cache)
    {
        _validator = validator;
        _readOnlyRepository = readOnlyRepository;
        _cache = cache;
    }

    public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = nameof(GetAllCustomerQuery);
        var customers = await _cache.GetOrCreateAsync(cacheKey, _readOnlyRepository.GetAllAsync);
        return Result.Success(customers);
    }

    public async Task<Result<CustomerQueryModel>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        var cacheKey = $"{nameof(GetCustomerByIdQuery)}_{request.Id}";

        // Obtendo o cliente da base.
        var customer = await _cache.GetOrCreateAsync(cacheKey, () => _readOnlyRepository.GetByIdAsync(request.Id));
        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Retornando o cliente no resultado.
        return Result.Success(customer);
    }
}