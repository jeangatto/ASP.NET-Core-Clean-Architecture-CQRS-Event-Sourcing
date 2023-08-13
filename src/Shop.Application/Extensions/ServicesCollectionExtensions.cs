using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Behaviors;

namespace Shop.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    /// <summary>
    /// Adds command handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return services
            .AddValidatorsFromAssembly(assembly)
            .AddMediatR(cfg => cfg
                .RegisterServicesFromAssembly(assembly)
                .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>)));
    }
}