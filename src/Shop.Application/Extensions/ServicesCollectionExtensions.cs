using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    private static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();

    public static void AddHandlers(this IServiceCollection services) =>
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ThisAssembly))
            .AddValidatorsFromAssembly(ThisAssembly);
}