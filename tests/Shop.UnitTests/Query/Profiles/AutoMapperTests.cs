using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Shop.Query.Profiles;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Query.Profiles;

[UnitTest]
public class AutoMapperTests
{
    [Fact]
    public void Should_Mapper_ConfigurationIsValid()
    {
        // Arrange
        var loggerFactory = NSubstitute.Substitute.For<ILoggerFactory>();

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EventToQueryModelProfile>(), loggerFactory));

        // Act
        // REF: https://fluentassertions.com/exceptions/
        var act = mapper.ConfigurationProvider.AssertConfigurationIsValid;

        // Assert
        act.Should().NotThrow();
    }
}