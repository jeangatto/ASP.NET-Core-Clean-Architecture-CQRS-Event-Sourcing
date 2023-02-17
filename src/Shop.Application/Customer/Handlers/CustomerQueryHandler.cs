using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Customer.Queries;
using Shop.Application.Customer.Validators;
using Shop.Domain.Interfaces.ReadOnly;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Handlers;

public class CustomerQueryHandler :
    IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>,
    IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
{
    private readonly GetCustomerByIdQueryValidator _validator;
    private readonly ICustomerReadOnlyRepository _readOnlyRepository;

    public CustomerQueryHandler(
        GetCustomerByIdQueryValidator validator,
        ICustomerReadOnlyRepository readOnlyRepository)
    {
        _validator = validator;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        => Result.Success(await _readOnlyRepository.GetAllAsync());

    public async Task<Result<CustomerQueryModel>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Retorna o resultado com os erros da validação.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Obtendo o cliente da base.
        var customerQueryModel = await _readOnlyRepository.GetByIdAsync(request.Id);
        if (customerQueryModel == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Retornando o cliente no resultado.
        return Result.Success(customerQueryModel);
    }
}