using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shop.PublicApi.Middlewares;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.PublicApi.Middlewares;

[UnitTest]
public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task Invoke_WhenNextSucceeds_PassesThroughResponse()
    {
        var environment = Substitute.For<IHostEnvironment>();
        var middleware = new ErrorHandlingMiddleware(
            context =>
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            },
            Substitute.For<ILogger<ErrorHandlingMiddleware>>(),
            environment);
        var context = new DefaultHttpContext();

        await middleware.Invoke(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task Invoke_WhenNextThrowsInDevelopment_ReturnsExceptionDetails()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Development);
        var middleware = CreateThrowingMiddleware(environment);
        var context = CreateContext();

        await middleware.Invoke(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("text/plain");
        (await ReadResponseAsync(context)).Should().Contain("expected failure");
    }

    [Fact]
    public async Task Invoke_WhenNextThrowsOutsideDevelopment_ReturnsGenericJsonError()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Production);
        var middleware = CreateThrowingMiddleware(environment);
        var context = CreateContext();

        await middleware.Invoke(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/json");
        var response = await ReadResponseAsync(context);
        response.Should().Contain("An internal error occurred while processing your request.");
        response.Should().NotContain("expected failure");
    }

    private static ErrorHandlingMiddleware CreateThrowingMiddleware(IHostEnvironment environment) =>
        new(
            _ => Task.FromException(new InvalidOperationException("expected failure")),
            Substitute.For<ILogger<ErrorHandlingMiddleware>>(),
            environment);

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponseAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
