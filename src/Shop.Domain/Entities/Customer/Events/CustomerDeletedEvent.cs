using System;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities.Customer.Events;

/// <summary>
/// Evento que representa um cliente deletado.
/// </summary>
public class CustomerDeletedEvent : CustomerBaseEvent
{
    public CustomerDeletedEvent(
        Guid id,
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth) : base(id, firstName, lastName, gender, email, dateOfBirth)
    {
    }
}