using FluentValidation;
using Shop.Application.Commands;
using Shop.Domain.Enums;

namespace Shop.Application.Customer.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Gender)
            .NotEmpty()
            .IsEnumName(typeof(EGender), caseSensitive: false);

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();
    }
}