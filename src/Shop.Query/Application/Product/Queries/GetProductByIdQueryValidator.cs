using FluentValidation;

namespace Shop.Query.Application.Product.Queries;

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}