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

public class UpdateProductCommandHandler(
    IValidator<UpdateProductCommand> validator,
    IProductWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductWriteOnlyRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<UpdateProductCommand> _validator = validator;

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result.Invalid(validationResult.AsErrors());
        }

        // Getting the Product from the database.
        var Product = await _repository.GetByIdAsync(request.Id);
        if (Product == null)
            return Result.NotFound($"No Product found by Id: {request.Id}");

        // Changing the price in the entity.
        Product.ChangePrice(request.Price);

        // Updating the entity in the repository.
        _repository.Update(Product);

        // Saving the changes to the database and firing events.
        await _unitOfWork.SaveChangesAsync();

        // Returning the success message.
        return Result.SuccessWithMessage("Updated successfully!");
    }
}