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
    private static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddValidatorsFromAssembly(ThisAssembly)
            .AddMediatR(cfg => cfg
                .RegisterServicesFromAssembly(ThisAssembly)
                .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>)));
}