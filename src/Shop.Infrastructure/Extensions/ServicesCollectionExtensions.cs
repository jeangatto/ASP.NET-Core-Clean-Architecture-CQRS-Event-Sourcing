using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.Shared;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Behaviors;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Repositories;
using Shop.Infrastructure.Data.Services;

namespace Shop.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static void AddMemoryCacheService(this IServiceCollection services)
        => services.AddScoped<ICacheService, MemoryCacheService>();

    public static void AddDistributedCacheService(this IServiceCollection services)
        => services.AddScoped<ICacheService, DistributedCacheService>();

    public static void AddInfrastructure(this IServiceCollection services)
    {
        // MediatR Pipelines
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Repositories
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        services.AddScoped<ICustomerWriteOnlyRepository, CustomerWriteOnlyRepository>();

        // Database Contexts
        services.AddScoped<WriteDbContext>();
        services.AddScoped<EventStoreDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}