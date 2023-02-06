using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Core.AppSettings;
using Shop.Infrastructure.Data.Context;

namespace Shop.PublicApi.Extensions;

internal static class ServicesCollectionExtensions
{
    public static IServiceCollection AddShopContext(this IServiceCollection services)
    {
        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

        services.AddDbContext<ShopContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ShopContext>>();
            var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>();
            var connectionString = connectionOptions.Value.ShopConnection;

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(migrationsAssembly);

                // Configurando a resiliência da conexão: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);

                // Log das tentativas de repetição
                options.LogTo(
                    filter: (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                    logger: eventData =>
                    {
                        if (eventData is not ExecutionStrategyEventData retryEventData) return;

                        var exceptions = retryEventData.ExceptionsEncountered;
                        var count = exceptions.Count;
                        var delay = retryEventData.Delay;
                        var message = exceptions[^1].Message;
                        logger.LogWarning("----- Retry #{Count} with delay {Delay} due to error: {Message}", count, delay, message);
                    });
            });

            // Quando for ambiente de desenvolvimento será logado informações detalhadas.
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
        });

        return services;
    }
}