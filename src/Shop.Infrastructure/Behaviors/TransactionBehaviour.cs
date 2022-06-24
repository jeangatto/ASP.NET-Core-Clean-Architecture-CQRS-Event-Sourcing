using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ITransaction _transaction;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

    public TransactionBehaviour(ITransaction transaction, ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var commandName = request.GetGenericTypeName();
        var response = default(TResponse);

        _logger.LogInformation("----- Starting handling transaction for {CommandName} ({@Command})", commandName, request);

        await _transaction.ExecuteAsync(async () => response = await next(), cancellationToken);

        _logger.LogInformation("----- Handling transaction completed for {CommandName} ({@Command})", commandName);

        return response;
    }
}