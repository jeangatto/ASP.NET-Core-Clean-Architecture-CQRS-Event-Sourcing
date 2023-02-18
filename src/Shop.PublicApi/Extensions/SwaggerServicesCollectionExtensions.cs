using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Shop.PublicApi.Extensions;

public static class SwaggerServicesCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Shop (e-commerce)",
                    Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, Princípios SOLID e Clean Architecture",
                    Contact = new OpenApiContact
                    {
                        Name = "Jean Gatto",
                        Email = "jean_gatto@hotmail.com",
                        Url = new Uri("https://www.linkedin.com/in/jeangatto/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE")
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, true);
        }).AddSwaggerGenNewtonsoftSupport();
    }
}