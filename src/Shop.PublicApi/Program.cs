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
        // Specify the default API Version as 1.0
        versioningOptions.DefaultApiVersion = ApiVersion.Default;
        // Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
        versioningOptions.ReportApiVersions = true;
        // If the client hasn't specified the API version in the request, use the default API version number
        versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddVersionedApiExplorer(explorerOptions =>
    {
        // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
        explorerOptions.GroupNameFormat = "'v'VVV";
        // NOTE: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        explorerOptions.SubstituteApiVersionInUrl = true;
    })
    .AddSwagger();

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
    // Enable the inclusion of a Server-Timing header in the HTTP response.
    options.EnableServerTimingHeader = true;
    // Track the opening and closing of database connections.
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