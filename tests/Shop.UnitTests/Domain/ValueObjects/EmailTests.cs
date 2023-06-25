using FluentAssertions;
using Shop.Domain.ValueObjects;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Domain.ValueObjects;

[UnitTest]
public class EmailTests
{
    [Theory]
    [InlineData("ma@hostname.com")]
    [InlineData("ma@hostname.comcom")]
    [InlineData("MA@hostname.coMCom")]
    [InlineData("MA@HOSTNAME.COM")]
    [InlineData("m.a@hostname.co")]
    [InlineData("m_a1a@hostname.com")]
    [InlineData("ma-a@hostname.com")]
    [InlineData("ma-a@hostname.com.edu")]
    [InlineData("ma-a.aa@hostname.com.edu")]
    [InlineData("ma.h.saraf.onemore@hostname.com.edu")]
    [InlineData("ma12@hostname.com")]
    [InlineData("12@hostname.com")]
    public void Should_ReturnsSuccess_When_CreateEmailIsValid(string emailAddress)
    {
        // Act
        var act = Email.Create(emailAddress);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().NotBeNull().And.BeOfType<Email>();
        act.Value.Address.Should().NotBeNullOrEmpty().And.Be(emailAddress.ToLowerInvariant());
    }

    [Theory]
    [InlineData("Abc.example.com")] // No `@`
    [InlineData("A@b@c@example.com")] // multiple `@`
    [InlineData("ma...ma@jjf.co")] // continuous multiple dots in name
    [InlineData("ma@jjf.c")] // only 1 char in extension
    [InlineData("ma@jjf..com")] // continuous multiple dots in domain
    [InlineData("ma@@jjf.com")] // continuous multiple `@`
    [InlineData("@majjf.com")] // nothing before `@`
    [InlineData("ma.@jjf.com")] // nothing after `.`
    [InlineData("ma_@jjf.com")] // nothing after `_`
    [InlineData("ma_@jjf")] // no domain extension
    [InlineData("ma_@jjf.")] // nothing after `_` and .
    [InlineData("ma@jjf.")] // nothing after `.`
    public void Should_ReturnsFail_When_CreateEmailInValid(string emailAddress)
    {
        // Act
        var act = Email.Create(emailAddress);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Value.Should().BeNull();
        act.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.HaveCount(1)
            .And.Satisfy(errorMessage => errorMessage == "O endereço de e-mail não é valido.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_ReturnsFail_When_CreateEmailIsEmptyOrNull(string emailAddress)
    {
        // Act
        var act = Email.Create(emailAddress);

        // Assert
        act.Should().NotBeNull();
        act.IsSuccess.Should().BeFalse();
        act.Value.Should().BeNull();
        act.Errors.Should().NotBeNullOrEmpty()
            .And.OnlyHaveUniqueItems()
            .And.HaveCount(1)
            .And.Satisfy(message => message == "O endereço de e-mail deve ser informado.");
    }
}