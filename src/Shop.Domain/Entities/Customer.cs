using System;
using Shop.Core.Abstractions;
using Shop.Core.Interfaces;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

public class Customer : BaseAuditEntity, IAggregateRoot
{
    public Customer(string firstName, string lastName, Gender gender, string email, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
    }

    private Customer()
    {
    }

    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public Gender Gender { get; private init; }
    public string Email { get; private init; }
    public DateTime DateOfBirth { get; private init; }
}