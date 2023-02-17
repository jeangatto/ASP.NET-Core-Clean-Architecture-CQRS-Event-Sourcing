using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.Application.Customer.Validators;
using Shop.Core.Interfaces;
using Shop.Domain.Interfaces.ReadOnly;
using Shop.Domain.Interfaces.WriteOnly;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class CustomerCommandHandler :
    IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>,
    IRequestHandler<UpdateCustomerCommand, Result>,
    IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly CreateCustomerCommandValidator _createCommandValidator;
    private readonly UpdateCustomerCommandValidator _updateCommandValidator;
    private readonly RemoveCustomerCommandValidator _removeCommandValidator;
    private readonly ICustomerWriteOnlyRepository _writeOnlyRepository;
    private readonly ICustomerReadOnlyRepository _readOnlyRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerCommandHandler(
        CreateCustomerCommandValidator createCommandValidator,
        UpdateCustomerCommandValidator updateCommandValidator,
        RemoveCustomerCommandValidator removeCommandValidator,
        ICustomerWriteOnlyRepository writeOnlyRepository,
        ICustomerReadOnlyRepository readOnlyRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _createCommandValidator = createCommandValidator;
        _updateCommandValidator = updateCommandValidator;
        _removeCommandValidator = removeCommandValidator;
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatedCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _createCommandValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        // Instanciando o VO Email.
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

        // Adicionando a entidade no repositório.
        _writeOnlyRepository.Add(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando o ID e a mensagem de sucesso.
        return Result.Success(new CreatedCustomerResponse(customer.Id), "Cadastrado com sucesso!");
    }

    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _updateCommandValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        // Obtendo o cliente da base.
        var customerQueryModel = await _readOnlyRepository.GetByIdAsync(request.Id);
        if (customerQueryModel == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        // Instanciando o VO Email.
        var newEmail = new Email(request.Email);

        // Verificiando se já existe um cliente com o endereço de e-mail.
        if (await _writeOnlyRepository.ExistsByEmailAsync(newEmail, customerQueryModel.Id))
        {
            // Retorna o resultado com o erro informado:
            return Result.Error("O endereço de e-mail informado já está sendo utilizado.");
        }

        var customer = _mapper.Map<Domain.Entities.Customer.Customer>(customerQueryModel);
        customer.ChangeEmail(newEmail);

        // Atualizando a entidade no repositório.
        _writeOnlyRepository.Update(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando a mensagem de sucesso.
        return Result.SuccessWithMessage("Atualizado com sucesso!");
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validanto a requisição.
        var validationResult = await _removeCommandValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors()); // Retorna o resultado com os erros da validação.

        // Obtendo o cliente da base.
        var customerQueryModel = await _readOnlyRepository.GetByIdAsync(request.Id);
        if (customerQueryModel == null)
            return Result.NotFound($"Nenhum cliente encontrado pelo Id: {request.Id}");

        var customer = _mapper.Map<Domain.Entities.Customer.Customer>(customerQueryModel);
        customer.Delete();

        // Removendo a entidade no repositório.
        _writeOnlyRepository.Remove(customer);

        // Salvando as alterações no banco e disparando os eventos.
        await _unitOfWork.SaveChangesAsync();

        // Retornando a mensagem de sucesso.
        return Result.SuccessWithMessage("Removido com sucesso!");
    }
}