using FluentValidation;
using Shop.Application.Commands;

namespace Shop.Application.Customer.Validators;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();
    }
}