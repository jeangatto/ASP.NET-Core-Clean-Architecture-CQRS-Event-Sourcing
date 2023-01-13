using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;

namespace Shop.Core;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddOptions<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        return services;
    }

    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, string configSectionPath)
        where TOptions : BaseOptions
    {
        return services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath, options => options.BindNonPublicProperties = true)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
}