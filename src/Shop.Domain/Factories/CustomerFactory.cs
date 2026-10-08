using System;
using System.Linq;
using Ardalis.Result;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Factories;

public static class CustomerFactory
{
    private const int MaxNameLength = 100;
    private const int MaxLastNameLength = 100;

    public static Result<Customer> Create(
        string firstName,
        string lastName,
        EGender gender,
        string email,
        DateTime dateOfBirth)
    {
        var trimmedFirstName = firstName?.Trim();
        var trimmedLastName = lastName?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedFirstName))
            return Result<Customer>.Error("The first name must be provided.");

        if (string.IsNullOrWhiteSpace(trimmedLastName))
            return Result<Customer>.Error("The last name must be provided.");

        if (trimmedFirstName.Length > MaxNameLength)
            return Result<Customer>.Error($"The first name exceeds the maximum length of {MaxNameLength} characters.");

        if (trimmedLastName.Length > MaxLastNameLength)
            return Result<Customer>.Error($"The last name exceeds the maximum length of {MaxLastNameLength} characters.");

        var emailResult = Email.Create(email);
        return !emailResult.IsSuccess
            ? Result<Customer>.Error(new ErrorList(emailResult.Errors.ToArray()))
            : Result<Customer>.Success(new Customer(trimmedFirstName, trimmedLastName, gender, emailResult.Value, dateOfBirth));
    }

    public static Customer Create(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
        => new(firstName.Trim(), lastName.Trim(), gender, email, dateOfBirth);
}