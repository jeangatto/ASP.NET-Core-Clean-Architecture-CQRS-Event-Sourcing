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
    private static readonly Action<BinderOptions> ConfigureBinderOptions = options =>
        options.BindNonPublicProperties = true;

    public static void AddCorrelationGenerator(this IServiceCollection services) =>
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

    public static void ConfigureAppSettings(this IServiceCollection services) =>
        services
            .AddOptionsWithValidation<ConnectionOptions>()
            .AddOptionsWithValidation<CacheOptions>();

    /// <summary>
    /// Adds options with validation to the service collection.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to add.</typeparam>
    /// <param name="services">The service collection.</param>
    private static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services)
        where TOptions : class, IAppOptions
    {
        return services
            .AddOptions<TOptions>()
            .BindConfiguration(TOptions.ConfigSectionPath, ConfigureBinderOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
}