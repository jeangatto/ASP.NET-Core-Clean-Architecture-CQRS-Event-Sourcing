using FluentValidation;
using Shop.Application.Customer.Queries;

namespace Shop.Application.Customer.Validators;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}