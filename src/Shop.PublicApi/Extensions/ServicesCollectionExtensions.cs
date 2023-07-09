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
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Extensions;

namespace Shop.PublicApi.Extensions;

[ExcludeFromCodeCoverageAttribute]
internal static class ServicesCollectionExtensions
{
    private const string MigrationsAssembly = "Shop.PublicApi";
    private static readonly string[] DatabaseTags = { "database" };

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Shop (e-commerce)",
                Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture",
                Contact = new OpenApiContact
                {
                    Name = "Jean Gatto",
                    Email = "jean_gatto@hotmail.com",
#pragma warning disable S1075 // Refactor your code not to use hardcoded absolute paths or URIs.
                    Url = new Uri("https://www.linkedin.com/in/jeangatto/")
#pragma warning restore S1075 // Refactor your code not to use hardcoded absolute paths or URIs.
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
#pragma warning disable S1075 // Refactor your code not to use hardcoded absolute paths or URIs.
                    Url = new Uri("https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE")
#pragma warning restore S1075 // Refactor your code not to use hardcoded absolute paths or URIs.
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, true);
        });

        services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionOptions = configuration.GetOptions<ConnectionOptions>();

        var healthCheckBuilder = services
            .AddHealthChecks()
            .AddDbContextCheck<WriteDbContext>(tags: DatabaseTags)
            .AddDbContextCheck<EventStoreDbContext>(tags: DatabaseTags)
            .AddMongoDb(connectionOptions.NoSqlConnection, tags: DatabaseTags);

        if (!connectionOptions.CacheConnection.IsInMemoryCache())
            healthCheckBuilder.AddRedis(connectionOptions.CacheConnection);
    }

    public static void AddShopDbContext(this IServiceCollection services) =>
        services.AddDbContext<WriteDbContext>((serviceProvider, optionsBuilder) =>
            ConfigureDbContext<WriteDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.TrackAll));

    public static void AddEventDbContext(this IServiceCollection services) =>
        services.AddDbContext<EventStoreDbContext>((serviceProvider, optionsBuilder) =>
            ConfigureDbContext<EventStoreDbContext>(serviceProvider, optionsBuilder,
                QueryTrackingBehavior.NoTrackingWithIdentityResolution));

    public static void AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionOptions = configuration.GetOptions<ConnectionOptions>();
        if (connectionOptions.CacheConnection.IsInMemoryCache())
        {
            services
                .AddMemoryCache() // ASP.NET Core Memory Cache.
                .AddMemoryCacheService();// Shop Infra Service.
        }
        else
        {
            // ASP.NET Core Redis Distributed Cache.
            // REF: https://learn.microsoft.com/pt-br/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = "master";
                options.Configuration = connectionOptions.CacheConnection;
            }).AddDistributedCacheService(); // Shop Infra Service.
        }
    }

    private static bool IsInMemoryCache(this string connection) =>
        connection.Equals("InMemory", StringComparison.InvariantCultureIgnoreCase);

    private static void ConfigureDbContext<TContext>(
        IServiceProvider serviceProvider,
        DbContextOptionsBuilder optionsBuilder,
        QueryTrackingBehavior queryTrackingBehavior) where TContext : DbContext
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var connectionOptions = serviceProvider.GetOptions<ConnectionOptions>();

        optionsBuilder.UseSqlServer(connectionOptions.SqlConnection, sqlServerOptions =>
            sqlServerOptions
                .MigrationsAssembly(MigrationsAssembly)
                .EnableRetryOnFailure(3) // Configure connection resiliency with retry on failure.
                .CommandTimeout(60)).UseQueryTrackingBehavior(queryTrackingBehavior);

        // Log retry attempts for execution strategy.
        optionsBuilder.LogTo(
            (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
            eventData =>
            {
                if (eventData is not ExecutionStrategyEventData retryEventData)
                    return;

                var exceptions = retryEventData.ExceptionsEncountered;

                // Log a warning message with the retry count, delay, and the error message of the last exception encountered.
                logger.LogWarning(
                    "----- DbContext: Retry #{Count} with delay {Delay} due to error: {Message}",
                    exceptions.Count,
                    retryEventData.Delay,
                    exceptions[^1].Message);
            });

        // Enable detailed errors and sensitive data logging if the environment is "development".
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (environment.IsDevelopment())
        {
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}