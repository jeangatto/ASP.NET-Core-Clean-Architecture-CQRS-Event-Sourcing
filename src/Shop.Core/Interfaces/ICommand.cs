using MediatR;

namespace Shop.Core.Interfaces;

/// <summary>
/// Interface marcadora para representar um comando com uma resposta vazia.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Interface marcadora para representar um comando com uma resposta.
/// </summary>
/// <typeparam name="TResponse">O tipo de resposta.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}