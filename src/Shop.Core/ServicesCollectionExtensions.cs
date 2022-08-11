using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;

namespace Shop.Core;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddOptions<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        return services;
    }

    private static void AddOptions<T>(this IServiceCollection services, string configSectionPath) where T : class
        => services
            .AddOptions<T>()
            .BindConfiguration(configSectionPath, options => options.BindNonPublicProperties = true)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}