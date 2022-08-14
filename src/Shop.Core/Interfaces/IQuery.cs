using MediatR;

namespace Shop.Core.Interfaces;

/// <summary>
/// Interface marcadora para representar um comando de leitura com uma resposta.
/// </summary>
/// <typeparam name="TResponse">O tipo de resposta.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}