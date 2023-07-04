using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Shop.Core.SharedKernel.Correlation;

namespace Shop.PublicApi.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        var correlationId = GetCorrelationId(context, correlationIdGenerator);
        AddcorrelationIdHeader(context, correlationId);
        await _next(context);
    }

    private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }
        else
        {
            return correlationIdGenerator.Get();
        }
    }

    private static void AddcorrelationIdHeader(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdHeader, new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder) =>
        builder.UseMiddleware<CorrelationIdMiddleware>();
}