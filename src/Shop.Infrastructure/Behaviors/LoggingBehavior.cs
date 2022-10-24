using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;

namespace Shop.Infrastructure.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commandName = request.GetGenericTypeName();

        _logger.LogInformation(
            "----- Handling command {CommandName} ({Command})",
            commandName, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();

        _logger.LogInformation(
            "----- Command {CommandName} handled ({TimeTaken} seconds) - response: {Response} ",
            commandName, timer.Elapsed.Seconds, response);

        return response;
    }
}