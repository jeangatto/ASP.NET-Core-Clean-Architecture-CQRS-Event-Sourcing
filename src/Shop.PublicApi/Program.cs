using System.Globalization;
using System.IO.Compression;
using FluentValidation;
using FluentValidation.Resources;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.Application.Extensions;
using Shop.Core.Extensions;
using Shop.Infrastructure.Extensions;
using Shop.PublicApi.Extensions;
using Shop.Query.Extensions;
using StackExchange.Profiling;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<GzipCompressionProviderOptions>(compressionOptions => compressionOptions.Level = CompressionLevel.Fastest)
    .Configure<JsonOptions>(jsonOptions => jsonOptions.JsonSerializerOptions.Configure())
    .Configure<RouteOptions>(routeOptions => routeOptions.LowercaseUrls = true);

builder.Services
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddResponseCompression(compressionOptions =>
    {
        compressionOptions.EnableForHttps = true;
        compressionOptions.Providers.Add<GzipCompressionProvider>();
    })
    .AddEndpointsApiExplorer()
    .AddApiVersioning(versioningOptions =>
    {
        versioningOptions.DefaultApiVersion = ApiVersion.Default;
        versioningOptions.ReportApiVersions = true;
        versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddVersionedApiExplorer(explorerOptions =>
    {
        explorerOptions.GroupNameFormat = "'v'VVV";
        explorerOptions.SubstituteApiVersionInUrl = true;
    })
    .AddSwagger(builder.Configuration);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(behaviorOptions =>
    {
        behaviorOptions.SuppressMapClientErrors = true;
        behaviorOptions.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(_ => { });

// Adding the application services in ASP.NET Core DI.
builder.Services
    .ConfigureAppSettings()
    .AddCorrelationGenerator()
    .AddInfrastructure()
    .AddCommandHandlers()
    .AddQueryHandlers()
    .AddWriteDbContext()
    .AddWriteOnlyRepositories()
    .AddReadDbContext()
    .AddReadOnlyRepositories()
    .AddCacheService(builder.Configuration)
    .AddHealthChecks(builder.Configuration);

// MiniProfiler for .NET
// https://miniprofiler.com/dotnet/
builder.Services.AddMiniProfiler(options =>
{
    // Route: /profiler/results-index
    options.RouteBasePath = "/profiler";
    options.ColorScheme = ColorScheme.Dark;
    options.EnableServerTimingHeader = true;
    options.TrackConnectionOpenClose = true;
    options.EnableDebugMode = builder.Environment.IsDevelopment();
}).AddEntityFramework();

// Validating the services added in the ASP.NET Core DI.
builder.Host.UseDefaultServiceProvider((context, serviceProviderOptions) =>
{
    serviceProviderOptions.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    serviceProviderOptions.ValidateOnBuild = true;
});

// Using the Kestrel Server (linux).
builder.WebHost.UseKestrel(kestrelOptions => kestrelOptions.AddServerHeader = false);

// FluentValidation global configuration.
ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;
ValidatorOptions.Global.LanguageManager = new LanguageManager { Enabled = true, Culture = new CultureInfo("en-US") };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseErrorHandling();
app.UseSwagger();
app.UseSwaggerUI();
app.UseResponseCompression();
app.UseHttpLogging();
app.UseHttpsRedirection();
app.UseMiniProfiler();
app.UseCorrelationId();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAppAsync();