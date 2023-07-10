using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.Application;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(executingAssembly))
            .AddValidatorsFromAssembly(executingAssembly);
    }
}