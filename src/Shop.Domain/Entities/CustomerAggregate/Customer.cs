using System;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Entities.CustomerAggregate;

public class Customer : BaseEntity, IAggregateRoot
{
    private bool _isDeleted;

    /// <summary>
    /// Initializes a new instance of the Customer class.
    /// </summary>
    /// <param name="firstName">The first name of the customer.</param>
    /// <param name="lastName">The last name of the customer.</param>
    /// <param name="gender">The gender of the customer.</param>
    /// <param name="email">The email address of the customer.</param>
    /// <param name="dateOfBirth">The date of birth of the customer.</param>
    public Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;

        AddDomainEvent(new CustomerCreatedEvent(Id, firstName, lastName, gender, email.Address, dateOfBirth));
    }

    /// <summary>
    /// Default constructor for Entity Framework or other ORM frameworks.
    /// </summary>
    public Customer()
    {
    }

    // Properties
    /// <summary>
    /// Gets the first name of the customer.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Gets the last name of the customer.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Gets the gender of the customer.
    /// </summary>
    public EGender Gender { get; }

    /// <summary>
    /// Gets or sets the email address of the customer.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the date of birth of the customer.
    /// </summary>
    public DateTime DateOfBirth { get; }

    /// <summary>
    /// Changes the email address of the customer.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    public void ChangeEmail(Email newEmail)
    {
        if (Email.Equals(newEmail))
            return;

        Email = newEmail;

        AddDomainEvent(new CustomerUpdatedEvent(Id, FirstName, LastName, Gender, newEmail.Address, DateOfBirth));
    }

    /// <summary>
    /// Deletes the customer.
    /// </summary>
    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;
        AddDomainEvent(new CustomerDeletedEvent(Id, FirstName, LastName, Gender, Email.Address, DateOfBirth));
    }
}
