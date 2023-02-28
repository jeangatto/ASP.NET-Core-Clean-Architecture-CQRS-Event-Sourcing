using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Core.Abstractions;
using Shop.Domain.Entities.CustomerAggregate.Repositories;

namespace Shop.Application.Customer.Handlers;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly DeleteCustomerCommandValidator _validator;
    private readonly ICustomerWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        DeleteCustomerCommandValidator validator,
        ICustomerWriteOnlyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        // Obtendo o cliente da base.
        var customer = await _repository.GetByIdAsync(request.Id);
        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Efetuando a removeção na entidade.
        customer.Delete();

        // Removendo a entidade no repositório.
        _repository.Remove(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando a mensagem de sucesso.
        return Result.SuccessWithMessage("Removido com sucesso!");
    }
}