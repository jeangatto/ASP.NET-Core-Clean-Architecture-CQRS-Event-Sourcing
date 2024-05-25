using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class UpdateCustomerCommandHandler(
    IValidator<UpdateCustomerCommand> validator,
    ICustomerWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Getting the customer from the database.
        var customer = await repository.GetByIdAsync(request.Id);
        if (customer == null)
            return Result.NotFound($"No customer found by Id: {request.Id}");

        // Instantiating the Email value object.
        var emailResult = Email.Create(request.Email);
        if (!emailResult.IsSuccess)
            return Result.Error(new ErrorList(emailResult.Errors.ToArray()));

        // Checking if there is already a customer with the email address.
        if (await repository.ExistsByEmailAsync(emailResult.Value, customer.Id))
            return Result.Error("The provided email address is already in use.");

        // Changing the email in the entity.
        customer.ChangeEmail(emailResult.Value);

        // Updating the entity in the repository.
        repository.Update(customer);

        // Saving the changes to the database and firing events.
        await unitOfWork.SaveChangesAsync();

        // Returning the success message.
        return Result.SuccessWithMessage("Updated successfully!");
    }
}