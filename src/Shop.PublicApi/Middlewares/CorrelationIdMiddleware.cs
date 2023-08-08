using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Shop.Core.SharedKernel.Correlation;

namespace Shop.PublicApi.Middlewares;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderKey = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        var correlationId = GetCorrelationId(context, correlationIdGenerator);
        AddcorrelationIdHeader(context, correlationId);
        await _next(context);
    }

    private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }

        return correlationIdGenerator.Get();
    }

    private static void AddcorrelationIdHeader(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdHeaderKey, new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });
    }
}