using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;

namespace Shop.Core;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddCustomOptions<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        return services;
    }

    public static void AddCustomOptions<TOptions>(this IServiceCollection services, string configSectionPath) where TOptions : BaseOptions
        => services
            .AddOptions<TOptions>()
            .BindConfiguration(configSectionPath, options => options.BindNonPublicProperties = true)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}