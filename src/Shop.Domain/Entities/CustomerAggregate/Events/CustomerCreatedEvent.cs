using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

/// <summary>
/// Evento que represente um novo cliente.
/// </summary>
public class CustomerCreatedEvent : CustomerBaseEvent
{
    public CustomerCreatedEvent(
        Guid id,
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth) : base(id, firstName, lastName, gender, email, dateOfBirth)
    {
    }
}