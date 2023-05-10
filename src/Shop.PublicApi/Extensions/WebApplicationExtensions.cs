using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Infrastructure.Data.Context;
using Shop.Query.Abstractions;

namespace Shop.PublicApi.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app, AsyncServiceScope serviceScope)
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
            app.Logger.LogError(ex, "Ocorreu uma exceção ao iniciar a aplicação: {Message}", ex.Message);
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<TContext>(this WebApplication app, TContext context)
        where TContext : DbContext
    {
        var dbName = context.Database.GetDbConnection().Database;

        app.Logger.LogInformation("----- {DbName}: {DbConnection}", dbName, context.Database.GetConnectionString());
        app.Logger.LogInformation("----- {DbName}: Verificando se existem migrações pendentes...", dbName);

        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            app.Logger.LogInformation("----- {DbName}: Criando e migrando a base de dados...", dbName);

            await context.Database.MigrateAsync();

            app.Logger.LogInformation("----- {DbName}: Base de dados criada e migrada com sucesso!", dbName);
        }
        else
        {
            app.Logger.LogInformation("----- {DbName}: Migrações estão em dia", dbName);
        }
    }

    private static async Task MigrateMongoDbContextAsync(this WebApplication app, IReadDbContext readDbContext)
    {
        app.Logger.LogInformation("----- MongoDB: {Connection}", readDbContext.ConnectionString);
        app.Logger.LogInformation("----- MongoDB: criando as coleções...");

        await readDbContext.CreateCollectionsAsync();

        app.Logger.LogInformation("----- MongoDB: coleções criadas com sucesso!");
    }
}