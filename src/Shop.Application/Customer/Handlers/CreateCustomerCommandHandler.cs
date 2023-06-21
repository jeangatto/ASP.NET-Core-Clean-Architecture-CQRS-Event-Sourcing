using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Factories;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    private readonly ICustomerWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerCommandHandler(
        IValidator<CreateCustomerCommand> validator,
        ICustomerWriteOnlyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatedCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Retorna o resultado com os erros da validação.
            return Result<CreatedCustomerResponse>.Invalid(validationResult.AsErrors());
        }

        // Instanciando o VO Email.
        var emailResult = Email.Create(request.Email);
        if (!emailResult.IsSuccess)
            return Result<CreatedCustomerResponse>.Error(emailResult.Errors.ToArray());

        // Verificiando se já existe um cliente com o endereço de e-mail.
        if (await _repository.ExistsByEmailAsync(emailResult.Value))
            return Result<CreatedCustomerResponse>.Error("O endereço de e-mail informado já está sendo utilizado.");

        // Criando a instancia da entidade cliente.
        // Ao instanciar será criado o evento: "CustomerCreatedEvent"
        var customer = CustomerFactory.Create(
            request.FirstName,
            request.LastName,
            request.Gender,
            emailResult.Value,
            request.DateOfBirth);

        // Adicionando a entidade no repositório.
        _repository.Add(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando o ID e a mensagem de sucesso.
        return Result<CreatedCustomerResponse>.Success(
            new CreatedCustomerResponse(customer.Id), "Cadastrado com sucesso!");
    }
}