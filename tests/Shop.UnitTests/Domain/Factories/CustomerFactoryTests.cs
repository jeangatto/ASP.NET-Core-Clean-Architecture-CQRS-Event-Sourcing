using System;
using FluentAssertions;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Factories;
using Shop.Domain.ValueObjects;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Domain.Factories;

[UnitTest]
public class CustomerFactoryTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnSuccessResult()
    {
        // Arrange
        const string firstName = "John";
        const string lastName = "Doe";
        const EGender gender = EGender.Male;
        const string email = "john.doe@example.com";
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var result = CustomerFactory.Create(firstName, lastName, gender, email, dateOfBirth);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.Gender.Should().Be(gender);
        result.Value.Email.Address.Should().Be(email);
        result.Value.DateOfBirth.Should().Be(dateOfBirth);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldReturnErrorResult()
    {
        // Arrange
        const string firstName = "John";
        const string lastName = "Doe";
        const EGender gender = EGender.Male;
        const string email = "invalid-email";
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var result = CustomerFactory.Create(firstName, lastName, gender, email, dateOfBirth);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_WithEmailValueObject_ShouldReturnCustomer()
    {
        // Arrange
        const string firstName = "John";
        const string lastName = "Doe";
        const EGender gender = EGender.Male;
        var email = Email.Create("john.doe@example.com").Value;
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var customer = CustomerFactory.Create(firstName, lastName, gender, email, dateOfBirth);

        // Assert
        customer.Should().NotBeNull();
        customer.FirstName.Should().Be(firstName);
        customer.LastName.Should().Be(lastName);
        customer.Gender.Should().Be(gender);
        customer.Email.Should().Be(email);
        customer.DateOfBirth.Should().Be(dateOfBirth);
    }

    [Fact]
    public void Create_WithWhitespaceNames_ShouldTrimAndReturnSuccessResult()
    {
        // Arrange
        const string firstName = "  John  ";
        const string lastName = "  Doe  ";
        const EGender gender = EGender.Male;
        const string email = "john.doe@example.com";
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var result = CustomerFactory.Create(firstName, lastName, gender, email, dateOfBirth);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }
}