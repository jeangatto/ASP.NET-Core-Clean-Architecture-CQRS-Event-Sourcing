using Microsoft.Extensions.DependencyInjection;
using Shop.Core.AppSettings;

namespace Shop.Core;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services)
    {
        services.AddOptions<ConnectionStrings>()
            .BindConfiguration(nameof(ConnectionStrings), options => options.BindNonPublicProperties = true);

        return services;
    }
}