using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Infrastructure;
using Shop.Infrastructure.Data.Context;

namespace Shop.PublicApi.Extensions;

public static class ServicesCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Shop (e-commerce)",
                    Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, Princípios SOLID e Clean Architecture",
                    Contact = new OpenApiContact
                    {
                        Name = "Jean Gatto",
                        Email = "jean_gatto@hotmail.com",
                        Url = new Uri("https://www.linkedin.com/in/jeangatto/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE")
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, true);
        });

        services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void AddShopContext(this IServiceCollection services)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<WriteDbContext>>();
            var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;

            options.UseSqlServer(connectionOptions.SqlConnection, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("Shop.PublicApi");

                // Configurando a resiliência da conexão.
                // REF: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);

                // Log das tentativas de repetição.
                options.LogTo(
                    filter: (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                    logger: eventData =>
                    {
                        if (eventData is not ExecutionStrategyEventData retryEventData) return;

                        var exceptions = retryEventData.ExceptionsEncountered;

                        logger.LogWarning(
                            "----- DbContext: Retry #{Count} with delay {Delay} due to error: {Message}",
                            exceptions.Count,
                            retryEventData.Delay,
                            exceptions[^1].Message);
                    });
            });

            // Quando o ambiente for o de "desenvolvimento" será logado informações detalhadas.
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
        });
    }

    public static void AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionOptions = configuration.GetOptions<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);
        var connection = connectionOptions.CacheConnection;

        if (connection.Equals("InMemory", StringComparison.InvariantCultureIgnoreCase))
        {
            // ASP.NET Core Memory Cache.
            services.AddMemoryCache();

            // Shop Infra Service.
            services.AddMemoryCacheService();
        }
        else
        {
            // ASP.NET Core Redis Distributed Cache.
            // REF: https://learn.microsoft.com/pt-br/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = "master";
                options.Configuration = connection;
            });

            // Shop Infra Service.
            services.AddDistributedCacheService();
        }
    }
}