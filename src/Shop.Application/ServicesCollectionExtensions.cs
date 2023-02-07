using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.Application;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        return services
            .AddMediatR(assemblies)
            .AddValidatorsFromAssemblies(assemblies);
    }
}