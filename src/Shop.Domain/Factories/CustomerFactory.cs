using System;
using Shop.Core.ValueObjects;
using Shop.Domain.Entities.CustomerAggregate;

namespace Shop.Domain.Factories;

public static class CustomerFactory
{
    public static Customer Create(string firstName, string lastName, EGender gender, string email, DateTime dateOfBirth)
        => new(firstName, lastName, gender, new Email(email), dateOfBirth);

    public static Customer Create(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
        => new(firstName, lastName, gender, email, dateOfBirth);
}