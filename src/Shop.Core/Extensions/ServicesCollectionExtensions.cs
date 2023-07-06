using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;
using Shop.Core.SharedKernel;
using Shop.Core.SharedKernel.Correlation;

namespace Shop.Core.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    private static readonly Action<BinderOptions> ConfigureBinderOptions
        = options => options.BindNonPublicProperties = true;

    public static void AddCorrelationGenerator(this IServiceCollection services) =>
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

    public static void ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddOptionsWithValidation<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        services.AddOptionsWithValidation<CacheOptions>(CacheOptions.ConfigSectionPath);
    }

    /// <summary>
    /// Adds options with validation to the service collection.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configSectionPath">The configuration section path.</param>
    private static void AddOptionsWithValidation<TOptions>(this IServiceCollection services, string configSectionPath)
        where TOptions : class, IAppOptions
    {
        // Adds options of type TOptions to the service collection.
        services
            .AddOptions<TOptions>()

            // Binds the configuration section specified by configSectionPath to the options.
            .BindConfiguration(configSectionPath, ConfigureBinderOptions)

            // Validates the options using data annotations.
            .ValidateDataAnnotations()

            // Performs additional validation on start.
            .ValidateOnStart();
    }
}