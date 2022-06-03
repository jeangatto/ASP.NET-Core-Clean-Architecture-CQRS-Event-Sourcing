using MediatR;

namespace Shop.Core.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}