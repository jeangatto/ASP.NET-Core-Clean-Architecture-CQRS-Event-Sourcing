using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Infrastructure;
using Shop.Infrastructure.Data.Context;

namespace Shop.PublicApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServicesCollectionExtensions
{
    private static readonly string[] DatabaseTags = { "database" };

    public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(swaggerOptions =>
        {
            swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Shop (e-commerce)",
                Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture",
                Contact = new OpenApiContact
                {
                    Name = "Jean Gatto",
                    Email = "jean_gatto@hotmail.com",
                    Url = new Uri(configuration.GetValue<string>("Urls:LinkedIn"))
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri(configuration.GetValue<string>("Urls:Github"))
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swaggerOptions.IncludeXmlComments(xmlPath, true);
        });
    }

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<ConnectionOptions>();

        var healthCheckBuilder = services
            .AddHealthChecks()
            .AddDbContextCheck<WriteDbContext>(tags: DatabaseTags)
            .AddDbContextCheck<EventStoreDbContext>(tags: DatabaseTags)
            .AddMongoDb(options.NoSqlConnection, tags: DatabaseTags);

        if (!options.CacheConnectionInMemory())
            healthCheckBuilder.AddRedis(options.CacheConnection);
    }

    public static IServiceCollection AddWriteDbContext(this IServiceCollection services)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, optionsBuilder) =>
            ConfigureDbContext<WriteDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.TrackAll));

        services.AddDbContext<EventStoreDbContext>((serviceProvider, optionsBuilder) =>
            ConfigureDbContext<EventStoreDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.NoTrackingWithIdentityResolution));

        return services;
    }

    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<ConnectionOptions>();
        if (options.CacheConnectionInMemory())
        {
            services
                .AddMemoryCache() // ASP.NET Core Memory Cache.
                .AddMemoryCacheService(); // Shop Infrastructure Service.
        }
        else
        {
            // ASP.NET Core Redis Distributed Cache.
            // REF: https://learn.microsoft.com/pt-br/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0
            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.InstanceName = "master";
                redisOptions.Configuration = options.CacheConnection;
            }).AddDistributedCacheService(); // Shop Infrastructure Service.
        }

        return services;
    }

    private static void ConfigureDbContext<TContext>(
        IServiceProvider serviceProvider,
        DbContextOptionsBuilder optionsBuilder,
        QueryTrackingBehavior queryTrackingBehavior) where TContext : DbContext
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var options = serviceProvider.GetOptions<ConnectionOptions>();

        optionsBuilder
            .UseSqlServer(options.SqlConnection, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                sqlServerOptions.EnableRetryOnFailure(3);
                sqlServerOptions.CommandTimeout(30);
            })
            .UseQueryTrackingBehavior(queryTrackingBehavior)
            .LogTo((eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying, eventData =>
            {
                if (eventData is not ExecutionStrategyEventData retryEventData)
                    return;

                var exceptions = retryEventData.ExceptionsEncountered;

                logger.LogWarning(
                    "----- DbContext: Retry #{Count} with delay {Delay} due to error: {Message}",
                    exceptions.Count,
                    retryEventData.Delay,
                    exceptions[^1].Message);
            });

        // Get the current hosting environment.
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (environment.IsDevelopment())
        {
            // Enable detailed errors for debugging purposes.
            optionsBuilder.EnableDetailedErrors();

            // Enable sensitive data logging for debugging purposes.
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}