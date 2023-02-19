using System;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.Application;
using Shop.Core;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Infrastructure;
using Shop.Infrastructure.Data.Context;
using Shop.PublicApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
builder.Services.Configure<MvcNewtonsoftJsonOptions>(options => options.SerializerSettings.Configure());
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(options =>
{
    // Specify the default API Version as 1.0
    options.DefaultApiVersion = ApiVersion.Default;

    // Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
    options.ReportApiVersions = true;

    // If the client hasn't specified the API version in the request, use the default API version number
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
    // NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    // NOTE: this option is only necessary when versioning by url segment. the SubstitutionFormat
    // can also be used to control the format of the API version in route templates
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwagger();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
        options.SuppressModelStateInvalidFilter = true;
    }).AddNewtonsoftJson();

// Adicionando os serviços da aplicação no ASP.NET Core DI.
builder.Services.ConfigureAppSettings();
builder.Services.AddInfrastructure();
builder.Services.AddMapperProfiles();
builder.Services.AddApplication();
builder.Services.AddShopContext();

var connectionOptions = builder.Configuration
    .GetSection(ConnectionOptions.ConfigSectionPath)
    .Get<ConnectionOptions>(binderOptions => binderOptions.BindNonPublicProperties = true);

var tags = new[] { "database" };
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<WriteDbContext>(tags: tags)
    .AddMongoDb(connectionOptions.EventConnection, tags: tags);

// Validando os serviços adicionados no ASP.NET Core DI.
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

// Utilizando o servidor Kestrel (linux).
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

// Configuração global do FluentValidation.
ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;
ValidatorOptions.Global.LanguageManager = new LanguageManager { Culture = new CultureInfo("pt-Br") };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = (context, healthReport) => context.Response.WriteAsync(healthReport.ToJson())
});
app.UseSwagger();
app.UseSwaggerUI();
app.UseResponseCompression();
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await using var serviceScope = app.Services.CreateAsyncScope();
await using var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();
var readDbContext = serviceScope.ServiceProvider.GetRequiredService<ReadDbContext>();
var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

try
{
    app.Logger.LogInformation("----- AutoMapper: Validando os mapeamentos...");

    mapper.ConfigurationProvider.AssertConfigurationIsValid();
    mapper.ConfigurationProvider.CompileMappings();

    app.Logger.LogInformation("----- AutoMapper: Mapeamentos são válidos!");

    app.Logger.LogInformation("----- SQL Server: {Connection}", writeDbContext.Database.GetConnectionString());
    app.Logger.LogInformation("----- SQL Server: Verificando se existem migrações pendentes...");

    if ((await writeDbContext.Database.GetPendingMigrationsAsync()).Any())
    {
        app.Logger.LogInformation("----- SQL Server: Criando e migrando a base de dados...");

        await writeDbContext.Database.MigrateAsync();

        app.Logger.LogInformation("----- SQL Server: Base de dados criada e migrada com sucesso!");
    }
    else
    {
        app.Logger.LogInformation("----- SQL Server: Migrações estão em dia.");
    }

    app.Logger.LogInformation("----- MongoDB: {Connection}", readDbContext.GetConnectionString());
    app.Logger.LogInformation("----- MongoDB: criando as coleções...");

    await readDbContext.CreateCollectionsAsync();

    app.Logger.LogInformation("----- MongoDB: coleções criadas com sucesso!");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Ocorreu uma exceção ao iniciar a aplicação: {Message}", ex.Message);
    throw;
}

app.Logger.LogInformation("----- Iniciando a aplicação...");
app.Run();