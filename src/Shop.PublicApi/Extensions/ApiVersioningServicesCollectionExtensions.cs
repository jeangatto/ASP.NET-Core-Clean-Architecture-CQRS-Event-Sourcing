using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Shop.PublicApi.Extensions;

public static class ApiVersioningServicesCollectionExtensions
{
    public static void AddApiVersioningAndApiExplorer(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // Specify the default API Version as 1.0
            options.DefaultApiVersion = ApiVersion.Default;

            // Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;

            // If the client hasn't specified the API version in the request, use the default API version number
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // NOTE: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });
    }
}