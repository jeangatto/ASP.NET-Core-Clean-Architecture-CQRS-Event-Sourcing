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
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<WriteDbContext>>();
            var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;

            options.UseSqlServer(connectionOptions.ShopConnection, sqlOptions =>
            {
                sqlOptions
                    .MigrationsAssembly("Shop.PublicApi")
                    // Configurando a resiliência da conexão: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                    .EnableRetryOnFailure(maxRetryCount: 3);

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

            // Quando o ambiente for o de "desenvolvimento" será logado informações detalhadas.
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
        });

        return services;
    }
}