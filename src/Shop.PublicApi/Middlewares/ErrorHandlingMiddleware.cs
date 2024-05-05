using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.PublicApi.Models;

namespace Shop.PublicApi.Middlewares;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    private const string ErrorMessage = "An internal error occurred while processing your request.";
    private static readonly string ApiResponseJson = ApiResponse.InternalServerError(ErrorMessage).ToJson();

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected exception was thrown: {Message}", ex.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (environment.IsDevelopment())
            {
                httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
                await httpContext.Response.WriteAsync(ex.ToString());
            }
            else
            {
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                await httpContext.Response.WriteAsync(ApiResponseJson);
            }
        }
    }
}