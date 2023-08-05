using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Repositories;
using Shop.Infrastructure.Data.Services;

namespace Shop.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtensions
{
    public static void AddMemoryCacheService(this IServiceCollection services) =>
        services.AddScoped<ICacheService, MemoryCacheService>();

    public static void AddDistributedCacheService(this IServiceCollection services) =>
        services.AddScoped<ICacheService, DistributedCacheService>();

    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreRepository, EventStoreRepository>()
            .AddScoped<ICustomerWriteOnlyRepository, CustomerWriteOnlyRepository>()
            .AddScoped<WriteDbContext>()
            .AddScoped<EventStoreDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>();
}