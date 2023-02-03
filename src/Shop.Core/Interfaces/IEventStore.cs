using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface IEventStore
{
    Task SaveAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
}