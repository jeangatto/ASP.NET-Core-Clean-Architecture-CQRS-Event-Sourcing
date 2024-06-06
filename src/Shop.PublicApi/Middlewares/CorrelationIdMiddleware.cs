using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Shop.Core.SharedKernel.Correlation;

namespace Shop.PublicApi.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderKey = "X-Correlation-Id";

    public async Task Invoke(HttpContext httpContext, ICorrelationIdGenerator correlationIdGenerator)
    {
        var correlationId = GetOrCreateCorrelationId(httpContext, correlationIdGenerator);

        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers.Append(CorrelationIdHeaderKey, correlationId);
            return Task.CompletedTask;
        });

        await next(httpContext);
    }

    private static string GetOrCreateCorrelationId(HttpContext httpContext, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }

        return correlationIdGenerator.Get();
    }
}