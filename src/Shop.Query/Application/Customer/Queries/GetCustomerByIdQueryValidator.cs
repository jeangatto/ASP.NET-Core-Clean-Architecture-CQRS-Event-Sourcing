using FluentValidation;

namespace Shop.Query.Application.Customer.Queries;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
