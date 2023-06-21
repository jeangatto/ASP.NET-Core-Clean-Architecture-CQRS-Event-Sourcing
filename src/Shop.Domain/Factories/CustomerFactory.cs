using System;
using System.Linq;
using Ardalis.Result;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Factories;

public static class CustomerFactory
{
    public static Result<Customer> Create(
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth)
    {
        var emailResult = Email.Create(email);
        if (!emailResult.IsSuccess)
            return Result.Error(emailResult.Errors.ToArray());

        return new Customer(firstName, lastName, gender, emailResult.Value, dateOfBirth);
    }

    public static Customer Create(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
        => new(firstName, lastName, gender, email, dateOfBirth);
}