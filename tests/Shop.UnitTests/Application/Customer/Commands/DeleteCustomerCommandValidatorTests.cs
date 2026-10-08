using System;
using FluentValidation.TestHelper;
using Shop.Application.Customer.Commands;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Application.Customer.Commands;

[UnitTest]
public class DeleteCustomerCommandValidatorTests
{
    private readonly DeleteCustomerCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Should_NotHaveError_WhenIdIsValid()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
