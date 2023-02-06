using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Commands;
using Shop.Application.Customer.Responses;
using Shop.Application.Customer.Validators;
using Shop.Core.Interfaces;
using Shop.Domain.Interfaces;

namespace Shop.Application.Customer.Handlers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    private readonly CreateCustomerCommandValidator _commandValidator;
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(
        CreateCustomerCommandValidator commandValidator,
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _commandValidator = commandValidator;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatedCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _commandValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Retorna o resultado com os erros da validação.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Verificiando se já existe um cliente com o endereço de e-mail.
        if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            // Retorna o resultado com o erro informado:
            return Result.Error("O endereço de e-mail informado já está sendo utilizado.");
        }

        // Criando a instancia da entidade cliente.
        // Ao instanciar será criado o evento: "CustomerCreatedEvent"
        var customer = new Domain.Entities.Customer.Customer(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.Email,
            request.DateOfBirth);

        // Adicionando a entidade cliente no repositório.
        _repository.Add(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.CommitAsync(cancellationToken);

        // Retornando o ID e a mensagem de sucesso.
        return Result.Success(new CreatedCustomerResponse(customer.Id), "Cadastrado com sucesso!");
    }
}