using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Infrastructure.Data.Context;
using Shop.Query.Abstractions;

namespace Shop.PublicApi.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task RunAppAsync(this WebApplication app)
    {
        await using var serviceScope = app.Services.CreateAsyncScope();

        var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

        app.Logger.LogInformation("----- AutoMapper: mappings are being validated...");

        // Validate the AutoMapper configuration by asserting that the mappings are valid
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        // Compile the AutoMapper mappings for better performance
        mapper.ConfigurationProvider.CompileMappings();

        app.Logger.LogInformation("----- AutoMapper: mappings are valid!");

        app.Logger.LogInformation("----- Databases are being migrated....");

        // Migrate the databases asynchronously using the provided service scope
        await app.MigrateDataBasesAsync(serviceScope);

        app.Logger.LogInformation("----- Databases have been successfully migrated!");

        app.Logger.LogInformation("----- Application is starting....");

        await app.RunAsync();
    }

    private static async Task MigrateDataBasesAsync(this WebApplication app, AsyncServiceScope serviceScope)
    {
        await using var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await using var eventStoreDbContext = serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        var readDbContext = serviceScope.ServiceProvider.GetRequiredService<IReadDbContext>();

        try
        {
            await app.MigrateDbContextAsync(writeDbContext);
            await app.MigrateDbContextAsync(eventStoreDbContext);
            await app.MigrateMongoDbContextAsync(readDbContext);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An exception occurred while initializing the application: {Message}", ex.Message);
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<TContext>(this WebApplication app, TContext context)
        where TContext : DbContext
    {
        var dbName = context.Database.GetDbConnection().Database;

        app.Logger.LogInformation("----- {DbName}: {DbConnection}", dbName, context.Database.GetConnectionString());
        app.Logger.LogInformation("----- {DbName}: checking if there are any pending migrations...", dbName);

        // Check if there are any pending migrations for the context.
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            app.Logger.LogInformation("----- {DbName}: creating and migrating the database...", dbName);

            await context.Database.MigrateAsync();

            app.Logger.LogInformation("----- {DbName}: database was created and migrated successfully", dbName);
        }
        else
        {
            app.Logger.LogInformation("----- {DbName}: all migrations are up to date", dbName);
        }
    }

    private static async Task MigrateMongoDbContextAsync(this WebApplication app, IReadDbContext readDbContext)
    {
        app.Logger.LogInformation("----- MongoDB: {Connection}", readDbContext.ConnectionString);
        app.Logger.LogInformation("----- MongoDB: collections are being created...");

        await readDbContext.CreateCollectionsAsync();

        app.Logger.LogInformation("----- MongoDB: collections were created successfully!");
    }
}