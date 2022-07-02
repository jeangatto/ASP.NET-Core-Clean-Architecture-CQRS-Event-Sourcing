using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Behaviors;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        services.AddScoped<ITransaction, ShopContextTransaction>();
        return services;
    }
}