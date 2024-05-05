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

public class CreateCustomerCommandHandler(
    IValidator<CreateCustomerCommand> validator,
    ICustomerWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    public async Task<Result<CreatedCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Return the result with validation errors.
            return Result<CreatedCustomerResponse>.Invalid(validationResult.AsErrors());
        }

        // Instantiating the Email value object.
        var email = Email.Create(request.Email).Value;

        // Checking if a customer with the email address already exists.
        if (await repository.ExistsByEmailAsync(email))
            return Result<CreatedCustomerResponse>.Error("The provided email address is already in use.");

        // Creating an instance of the customer entity.
        // When instantiated, the "CustomerCreatedEvent" will be created.
        var customer = CustomerFactory.Create(
            request.FirstName,
            request.LastName,
            request.Gender,
            email,
            request.DateOfBirth);

        // Adding the entity to the repository.
        repository.Add(customer);

        // Saving changes to the database and triggering events.
        await unitOfWork.SaveChangesAsync();

        // Returning the ID and success message.
        return Result<CreatedCustomerResponse>.Success(
            new CreatedCustomerResponse(customer.Id), "Successfully registered!");
    }
}