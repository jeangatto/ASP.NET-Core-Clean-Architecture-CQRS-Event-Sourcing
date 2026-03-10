using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

public sealed class CustomerUpdatedEvent(
    Guid id,
    string firstName,
    string lastName,
    EGender gender,
    string email,
    DateTime dateOfBirth) : CustomerBaseEvent(id, firstName, lastName, gender, email, dateOfBirth);