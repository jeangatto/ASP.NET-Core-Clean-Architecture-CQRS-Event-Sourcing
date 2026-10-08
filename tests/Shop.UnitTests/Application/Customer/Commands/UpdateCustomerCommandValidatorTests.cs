using System;
using FluentValidation.TestHelper;
using Shop.Application.Customer.Commands;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Application.Customer.Commands;

[UnitTest]
public class UpdateCustomerCommandValidatorTests
{
    private readonly UpdateCustomerCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateCustomerCommand { Id = Guid.Empty, Email = "john.doe@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new UpdateCustomerCommand { Id = Guid.NewGuid(), Email = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new UpdateCustomerCommand { Id = Guid.NewGuid(), Email = "invalid-email" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateCustomerCommand { Id = Guid.NewGuid(), Email = "john.doe@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
