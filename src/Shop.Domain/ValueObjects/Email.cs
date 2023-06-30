using Ardalis.Result;
using Shop.Core;

namespace Shop.Domain.ValueObjects;

public sealed record Email
{
    private Email(string address) =>
        Address = address.ToLowerInvariant().Trim();

    public Email() { } // Only for EF/ORM

    public string Address { get; }

    public static Result<Email> Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return Result<Email>.Error("O endereço de e-mail deve ser informado.");

        return !RegexPatterns.EmailIsValid.IsMatch(emailAddress)
            ? Result<Email>.Error("O endereço de e-mail não é valido.")
            : Result<Email>.Success(new Email(emailAddress));
    }

    public override string ToString() =>
        Address;
}