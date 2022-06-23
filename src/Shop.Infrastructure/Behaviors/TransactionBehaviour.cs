using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ITransaction _transaction;

    public TransactionBehaviour(ITransaction transaction) => _transaction = transaction;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = default(TResponse);

        await _transaction.ExecuteAsync(async () => response = await next(), cancellationToken);

        return response;
    }
}