using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;
using Shop.Core.SharedKernel;
using Shop.Core.SharedKernel.Correlation;

namespace Shop.Core.Extensions;

public static class ServicesCollectionExtensions
{
    private static readonly Action<BinderOptions> ConfigureBinderOptions
        = options => options.BindNonPublicProperties = true;

    public static void ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddOptionsWithValidation<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        services.AddOptionsWithValidation<CacheOptions>(CacheOptions.ConfigSectionPath);
    }

    public static void AddCorrelationGenerator(this IServiceCollection services) =>
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

    private static void AddOptionsWithValidation<TOptions>(this IServiceCollection services, string configSectionPath)
        where TOptions : class, IAppOptions
    {
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath, ConfigureBinderOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}