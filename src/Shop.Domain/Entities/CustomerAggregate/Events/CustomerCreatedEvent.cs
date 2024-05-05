using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

public class CustomerCreatedEvent(
    Guid id,
    string firstName,
    string lastName,
    EGender gender,
    string email,
    DateTime dateOfBirth) : CustomerBaseEvent(id, firstName, lastName, gender, email, dateOfBirth);