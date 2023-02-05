using System.Collections.Generic;
using Ardalis.Result;
using Shop.Core.Constants;
using Shop.Core.ValueObjects;

namespace Shop.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private Email(string address) => Address = address.ToLowerInvariant();

    private Email() { } // ORM

    public string Address { get; private init; }

    public static Result<Email> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result.Error("É necessário informar o endereço de e-mail.");

        if (!RegexPatterns.EmailRegexPattern.IsMatch(address))
            return Result.Error("O endereço de e-mail informado é inválido.");

        return Result.Success(new Email(address));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}