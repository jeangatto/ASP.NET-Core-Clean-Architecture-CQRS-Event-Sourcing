using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Shop.Application.Customer.Queries;
using Shop.Domain.Interfaces.ReadOnly;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Handlers;

public class CustomerQueryHandler :
    IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>,
    IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>
{
    private readonly ICustomerReadOnlyRepository _readOnlyRepository;

    public CustomerQueryHandler(ICustomerReadOnlyRepository readOnlyRepository)
        => _readOnlyRepository = readOnlyRepository;

    public async Task<Result<CustomerQueryModel>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _readOnlyRepository.GetByIdAsync(request.Id);
        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        return Result.Success(customer);
    }

    public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        => Result.Success(await _readOnlyRepository.GetAllAsync());
}