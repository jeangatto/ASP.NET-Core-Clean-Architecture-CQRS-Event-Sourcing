using FluentValidation;
using Shop.Application.Customer.Commands;

namespace Shop.Application.Customer.Validators;

public class RemoveCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public RemoveCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}