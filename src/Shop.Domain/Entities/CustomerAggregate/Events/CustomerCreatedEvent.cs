using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

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
