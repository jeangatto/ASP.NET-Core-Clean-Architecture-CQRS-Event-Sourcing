using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Core.Extensions;

[UnitTest]
public class ServicesCollectionExtensionsTests
{
    [Fact]
    public void Should_ReturnClassOptions_WhenConfigureAppSettings()
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

        var configuratio = configurationBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(_ => configuratio);
        services.ConfigureAppSettings();
        var serviceProvider = services.BuildServiceProvider(true);

        // Act
        var act = serviceProvider.GetRequiredService<IOptions<CacheOptions>>();

        // Assert
        act.Should().NotBeNull();
        act.Value.Should().NotBeNull();
        act.Value.AbsoluteExpirationInHours.Should().Be(absoluteExpirationInHours);
        act.Value.SlidingExpirationInSeconds.Should().Be(slidingExpirationInSeconds);
    }
}