using System.Collections.Generic;
using FluentAssertions;
using Shop.Core.Extensions;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Core.Extensions;

[UnitTest]
public class GenericTypeExtensionsTests
{
    [Fact]
    public void IsDefault_ShouldReturnTrue_ForDefaultValue()
    {
        // Arrange
        const int value = 0;

        // Act
        var result = value.IsDefault();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDefault_ShouldReturnFalse_ForNonDefaultValue()
    {
        // Arrange
        const int value = 5;

        // Act
        var result = value.IsDefault();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDefault_ShouldReturnTrue_ForNullReferenceValue()
    {
        // Arrange
        const string value = null;

        // Act
        var result = value.IsDefault();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetGenericTypeName_ShouldReturnTypeName_ForNonGenericType()
    {
        // Arrange
        const string obj = "test";

        // Act
        var result = obj.GetGenericTypeName();

        // Assert
        result.Should().Be("String");
    }

    [Fact]
    public void GetGenericTypeName_ShouldReturnGenericTypeName_ForGenericType()
    {
        // Arrange
        var obj = new List<string>();

        // Act
        var result = obj.GetGenericTypeName();

        // Assert
        result.Should().Be("List<String>");
    }
}