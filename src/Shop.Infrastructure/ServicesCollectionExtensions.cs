using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Behaviors;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Providers;

namespace Shop.Infrastructure;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ShopContext>();
        services.AddScoped<EventContext>();
        return services;
    }
}