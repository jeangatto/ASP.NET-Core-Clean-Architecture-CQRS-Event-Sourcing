using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Product.Commands;
using Shop.Application.Product.Responses;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.ProductAggregate;
using Shop.Domain.Factories;

namespace Shop.Application.Product.Handlers;

public class CreateProductCommandHandler(
    IValidator<CreateProductCommand> validator,
    IProductWriteOnlyRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<CreatedProductResponse>>
{
    private readonly IProductWriteOnlyRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<CreateProductCommand> _validator = validator;

    public async Task<Result<CreatedProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Return the result with validation errors.
            return Result<CreatedProductResponse>.Invalid(validationResult.AsErrors());
        }


        // Checking if a Product with the name already exists.
        if (await _repository.ExistsByNameAsync(request.Name))
            return Result<CreatedProductResponse>.Error("The provided product name is already in use.");

        // Creating an instance of the Product entity.
        // When instantiated, the "ProductCreatedEvent" will be created.
        var Product = ProductFactory.Create(
            request.Name,
            request.Description,
            request.Price);

        // Adding the entity to the repository.
        _repository.Add(Product);

        // Saving changes to the database and triggering events.
        await _unitOfWork.SaveChangesAsync();

        // Returning the ID and success message.
        return Result<CreatedProductResponse>.Success(
            new CreatedProductResponse(Product.Id), "Successfully added product!");
    }
}