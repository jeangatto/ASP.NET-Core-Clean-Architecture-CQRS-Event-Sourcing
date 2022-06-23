using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shop.Core.Interfaces;

namespace Shop.Infrastructure.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IDataBaseTransaction _dbTransaction;

    public TransactionBehaviour(IDataBaseTransaction dbTransaction) => _dbTransaction = dbTransaction;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = default(TResponse);

        await _dbTransaction.ExecuteAsync(async () => response = await next());

        return response;
    }
}