using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Behaviors;

namespace Shop.Infrastructure;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serivces)
    {
        serivces.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        serivces.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        return serivces;
    }
}
