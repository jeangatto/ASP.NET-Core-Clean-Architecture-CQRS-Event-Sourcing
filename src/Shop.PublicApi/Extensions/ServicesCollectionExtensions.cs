using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Infrastructure.Data;

namespace Shop.PublicApi.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddShopContext(this IServiceCollection services)
    {
        services.AddDbContext<ShopContext>((serviceProvider, optionsBuilder) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ShopContext>>();
            var connectionString = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value.ShopConnection;

            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);

                // Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);

                // Log the retry attempts
                optionsBuilder.LogTo(
                    filter: (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                    logger: (eventData) =>
                    {
                        var retryEventData = eventData as ExecutionStrategyEventData;
                        var exceptions = retryEventData.ExceptionsEncountered;
                        var count = exceptions.Count;
                        var delay = retryEventData.Delay;
                        var message = exceptions[exceptions.Count - 1]?.Message;
                        logger.LogWarning("Retry #{Count} with delay {Delay} due to error: {Message}", count, delay, message);
                    });
            });

            // When it is a development environment, detailed information will be logged.
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
                optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
        });

        return services;
    }
}