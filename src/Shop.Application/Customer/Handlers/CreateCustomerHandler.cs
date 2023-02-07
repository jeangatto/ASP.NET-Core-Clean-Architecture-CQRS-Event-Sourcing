using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using MediatR;
using Shop.Application.Commands;
using Shop.Application.Customer.Responses;
using Shop.Application.Customer.Validators;
using Shop.Core.Interfaces;
using Shop.Domain.Interfaces.WriteOnly;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    private readonly CreateCustomerCommandValidator _commandValidator;
    private readonly ICustomerWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(
        CreateCustomerCommandValidator commandValidator,
        ICustomerWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _commandValidator = commandValidator;
        _writeOnlyRepository = writeOnlyRepository;
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

        var email = new Email(request.Email);

        // Verificiando se já existe um cliente com o endereço de e-mail.
        if (await _writeOnlyRepository.ExistsByEmailAsync(email))
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
            email,
            request.DateOfBirth);

        // Adicionando a entidade cliente no repositório.
        _writeOnlyRepository.Add(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando o ID e a mensagem de sucesso.
        return Result.Success(new CreatedCustomerResponse(customer.Id), "Cadastrado com sucesso!");
    }
}