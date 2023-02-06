using System.Collections.Generic;
using Shop.Core.ValueObjects;

namespace Shop.Domain.ValueObjects;

public class Email : ValueObject
{
    public Email(string address)
        => Address = address.Trim().ToLowerInvariant();

    private Email() { } // ORM

    public string Address { get; private init; }

    public override string ToString() => Address;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}