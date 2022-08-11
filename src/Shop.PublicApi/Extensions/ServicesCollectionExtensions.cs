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

        services.AddDbContext<ShopContext>((serviceProvider, optionsBuilder) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ShopContext>>();
            var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;

            optionsBuilder.UseSqlServer(connectionOptions.ShopConnection, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(migrationsAssembly);

                // Configurando a resiliência da conexão: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);

                // Log tentativas de repetição
                optionsBuilder.LogTo(
                    filter: (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                    logger: (eventData) =>
                    {
                        var retryEventData = eventData as ExecutionStrategyEventData;
                        var exceptions = retryEventData.ExceptionsEncountered;
                        var count = exceptions.Count;
                        var delay = retryEventData.Delay;
                        var message = exceptions[exceptions.Count - 1]?.Message;
                        logger.LogWarning("----- Retry #{Count} with delay {Delay} due to error: {Message}", count, delay, message);
                    });
            });

            // NOTE: Quando for ambiente de desenvolvimento será logado informações detalhadas.
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
                optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
        });

        return services;
    }
}