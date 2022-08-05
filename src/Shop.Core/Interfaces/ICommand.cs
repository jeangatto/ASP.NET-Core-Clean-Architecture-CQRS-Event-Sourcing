using MediatR;

namespace Shop.Core.Interfaces;

/// <summary>
/// Interface marcadora para representar um comando de escrita com uma resposta vazia.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Interface marcadora para representar um comando de escrita com uma resposta.
/// </summary>
/// <typeparam name="TResponse">O tipo de resposta.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}