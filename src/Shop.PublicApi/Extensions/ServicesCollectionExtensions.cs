using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Data.Context;

namespace Shop.PublicApi.Extensions;

internal static class ServicesCollectionExtensions
{
    public static IServiceCollection AddShopContext(this IServiceCollection services)
    {
        services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        return services;
    }
}