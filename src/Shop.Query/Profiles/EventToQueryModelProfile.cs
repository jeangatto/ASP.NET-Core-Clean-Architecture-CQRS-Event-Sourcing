using AutoMapper;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.QueriesModel;

namespace Shop.Query.Profiles;

public class EventToQueryModelProfile : Profile
{
    public EventToQueryModelProfile()
    {
        CreateMap<CustomerCreatedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => Create(@event));

        CreateMap<CustomerUpdatedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => Create(@event));

        CreateMap<CustomerDeletedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => Create(@event));
    }

    public override string ProfileName => nameof(EventToQueryModelProfile);

    private static CustomerQueryModel Create<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent =>
        new(@event.Id, @event.FirstName, @event.LastName, @event.Gender.ToString(), @event.Email, @event.DateOfBirth);
}