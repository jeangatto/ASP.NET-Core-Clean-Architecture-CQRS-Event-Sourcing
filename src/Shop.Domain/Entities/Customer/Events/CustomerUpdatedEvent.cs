using System;
using Shop.Core.Abstractions;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities.Customer.Events;

public class CustomerUpdatedEvent : BaseDomainEvent
{
    public CustomerUpdatedEvent(
        Guid id,
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
    }

    public Guid Id { get; private init; }
    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public EGender Gender { get; private init; }
    public string Email { get; private init; }
    public DateTime DateOfBirth { get; private init; }
}