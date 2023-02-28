using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Core.Abstractions;
using Shop.Domain.Entities.CustomerAggregate.Repositories;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
{
    private readonly UpdateCustomerCommandValidator _validator;
    private readonly ICustomerWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        UpdateCustomerCommandValidator validator,
        ICustomerWriteOnlyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        // Obtendo o cliente da base.
        var customer = await _repository.GetByIdAsync(request.Id);
        if (customer == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Instanciando o VO Email.
        var newEmail = new Email(request.Email);

        // Verificiando se já existe um cliente com o endereço de e-mail.
        if (await _repository.ExistsByEmailAsync(newEmail, customer.Id))
            return Result.Error("O endereço de e-mail informado já está sendo utilizado.");

        // Efetuando a alteração do e-mail na entidade.
        customer.ChangeEmail(newEmail);

        // Atualizando a entidade no repositório.
        _repository.Update(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando a mensagem de sucesso.
        return Result.SuccessWithMessage("Atualizado com sucesso!");
    }
}