using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shop.Core.Interfaces;

namespace Shop.Application;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())

            // Command Handlers with Response
            .AddClasses(impl => impl.AssignableTo(typeof(ICommandHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            // Command Handlers
            .AddClasses(impl => impl.AssignableTo(typeof(ICommandHandler<>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            // Query Handlers with Response
            .AddClasses(impl => impl.AssignableTo(typeof(IQueryHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}