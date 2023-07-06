using System.Globalization;
using System.IO.Compression;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.Application;
using Shop.Core.Extensions;
using Shop.Infrastructure.Extensions;
using Shop.PublicApi.Extensions;
using Shop.PublicApi.Middlewares;
using Shop.Query.Extensions;
using StackExchange.Profiling;

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

// HealthChecks (API, EF Core, MongoDB, Redis)
builder.Services.AddHealthChecks(builder.Configuration);

// Adding the application services in ASP.NET Core DI.
builder.Services.ConfigureAppSettings();
builder.Services.AddCorrelationGenerator();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddQuery();
builder.Services.AddShopDbContext();
builder.Services.AddEventDbContext();
builder.Services.AddCacheService(builder.Configuration);

// MiniProfiler for .NET
// https://miniprofiler.com/dotnet/
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler"; // Route: /profiler/results-index
    options.ColorScheme = ColorScheme.Dark;
    options.EnableServerTimingHeader = true;
    options.TrackConnectionOpenClose = true;
    options.EnableDebugMode = builder.Environment.IsDevelopment();
}).AddEntityFramework();

// Validating the services added in the ASP.NET Core DI.
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

// Using the Kestrel server (linux).
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

// FluentValidation global configuration.
ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;
ValidatorOptions.Global.LanguageManager = new LanguageManager { Culture = new CultureInfo("pt-Br") };

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        Predicate = _ => true,
        AllowCachingResponses = false,
        ResponseWriter = (httpContext, healthReport) => httpContext.Response.WriteAsync(healthReport.ToJson())
    });

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

await using var serviceScope = app.Services.CreateAsyncScope();
var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();

app.Logger.LogInformation("----- AutoMapper: mappings are being validated...");

// Dry run all configured type maps and throw AutoMapper.AutoMapperConfigurationException for each problem.
mapper.ConfigurationProvider.AssertConfigurationIsValid();

// Compile all underlying mapping expressions to cached delegates.
mapper.ConfigurationProvider.CompileMappings();

app.Logger.LogInformation("----- AutoMapper: mappings are valid!");

app.Logger.LogInformation("----- Databases are being migrated....");

await app.MigrateDbAsync(serviceScope);

app.Logger.LogInformation("----- Databases have been successfully migrated!");

app.Logger.LogInformation("----- Application is starting....");
await app.RunAsync();