using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

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