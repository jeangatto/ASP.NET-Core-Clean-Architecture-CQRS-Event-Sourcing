using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Core.Extensions;

[UnitTest]
public class ConfigurationExtensionsTests
{
    [Fact]
    public void Should_ReturnsClassOptions_WhenGetOptions()
    {
        // Arrange
        const int absoluteExpirationInHours = 4;
        const int slidingExpirationInSeconds = 120;

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "CacheOptions:AbsoluteExpirationInHours", absoluteExpirationInHours.ToString() },
            { "CacheOptions:SlidingExpirationInSeconds", slidingExpirationInSeconds.ToString() }
        });

        var configuration = configurationBuilder.Build();

        // Act
        var act = configuration.GetOptions<CacheOptions>(CacheOptions.ConfigSectionPath);

        // Assert
        act.Should().NotBeNull();
        act.AbsoluteExpirationInHours.Should().Be(absoluteExpirationInHours);
        act.SlidingExpirationInSeconds.Should().Be(slidingExpirationInSeconds);
    }
}