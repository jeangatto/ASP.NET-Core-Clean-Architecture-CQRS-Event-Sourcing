using System;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities.Customer.Events;

/// <summary>
/// Evento que representa um atualização de um cliente.
/// </summary>
public class CustomerUpdatedEvent : CustomerBaseEvent
{
    public CustomerUpdatedEvent(
        Guid id,
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth) : base(id, firstName, lastName, gender, email, dateOfBirth)
    {
    }
}