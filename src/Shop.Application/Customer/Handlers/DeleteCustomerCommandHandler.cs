using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Application.Customer.Handlers;

public class DeleteCustomerCommandHandler(
    IValidator<DeleteCustomerCommand> validator,
    ICustomerWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Retrieving the customer from the database.
        var customer = await repository.GetByIdAsync(request.Id);
        if (customer == null)
            return Result.NotFound($"No customer found by Id: {request.Id}");

        // Marking the entity as deleted, the CustomerDeletedEvent will be added.
        customer.Delete();

        // Removing the entity from the repository.
        repository.Remove(customer);

        // Saving the changes to the database and triggering the events.
        await unitOfWork.SaveChangesAsync();

        // Returning the success message.
        return Result.SuccessWithMessage("Successfully removed!");
    }
}