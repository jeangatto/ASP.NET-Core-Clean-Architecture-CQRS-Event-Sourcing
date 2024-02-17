using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Product.Commands;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.ProductAggregate;

namespace Shop.Application.Product.Handlers;

public class DeleteProductCommandHandler(
    IValidator<DeleteProductCommand> validator,
    IProductWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductWriteOnlyRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<DeleteProductCommand> _validator = validator;

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Retrieving the Product from the database.
        var Product = await _repository.GetByIdAsync(request.Id);
        if (Product == null)
            return Result.NotFound($"No Product found by Id: {request.Id}");

        // Marking the entity as deleted, the ProductDeletedEvent will be added.
        Product.Delete();

        // Removing the entity from the repository.
        _repository.Remove(Product);

        // Saving the changes to the database and triggering the events.
        await _unitOfWork.SaveChangesAsync();

        // Returning the success message.
        return Result.SuccessWithMessage("Successfully removed!");
    }
}