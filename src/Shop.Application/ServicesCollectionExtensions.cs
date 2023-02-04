using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shop.Core.Interfaces;

namespace Shop.Application;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assemblies = new[] { Assembly.GetExecutingAssembly() };

        services
            .AddMediatR(assemblies)
            .AddValidatorsFromAssemblies(assemblies)

            // Adicionando todos os CommandHandler e QueryHandler
            .Scan(scan => scan.FromAssemblies(assemblies)
            // CommandHandler with Response
            .AddClasses(impl => impl.AssignableTo(typeof(ICommandHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            // CommandHandler
            .AddClasses(impl => impl.AssignableTo(typeof(ICommandHandler<>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            // QueryHandler with Response
            .AddClasses(impl => impl.AssignableTo(typeof(IQueryHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}