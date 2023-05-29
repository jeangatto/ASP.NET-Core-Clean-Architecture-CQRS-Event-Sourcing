namespace Shop.Domain.ValueObjects;

public sealed record Email
{
    public Email(string address)
        => Address = address.Trim().ToLowerInvariant();

    public string Address { get; }

    public override string ToString()
        => Address;
}