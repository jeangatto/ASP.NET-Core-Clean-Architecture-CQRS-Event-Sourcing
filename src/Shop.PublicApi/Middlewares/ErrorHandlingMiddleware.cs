using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.PublicApi.Models;

namespace Shop.PublicApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private const string ErrorMessage = "An internal error occurred while processing your request.";
    private static readonly string ApiResponseJson = ApiResponse.InternalServerError(ErrorMessage).ToJson();

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected exception was thrown: {Message}", ex.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (_environment.IsDevelopment())
            {
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsync(ex.ToString());
            }
            else
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(ApiResponseJson);
            }
        }
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static void UseErrorHandling(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}