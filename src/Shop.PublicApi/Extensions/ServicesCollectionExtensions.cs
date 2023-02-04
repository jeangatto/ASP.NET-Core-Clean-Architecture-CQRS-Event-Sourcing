using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Data.Context;

namespace Shop.PublicApi.Extensions;

internal static class ServicesCollectionExtensions
{
    public static IServiceCollection AddShopContext(this IServiceCollection services)
    {
        services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase(nameof(ShopContext)));
        return services;
    }

    public static IServiceCollection AddEventContext(this IServiceCollection services)
    {
        services.AddDbContext<EventContext>(options => options.UseInMemoryDatabase(nameof(EventContext)));
        return services;
    }
}