using AutoMapper;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.QueriesModel;

namespace Shop.Query.Profiles;

public class EventToQueryModelProfile : Profile
{
    public EventToQueryModelProfile()
    {
        CreateMap<CustomerCreatedEvent, CustomerQueryModel>(MemberList.Destination);
        CreateMap<CustomerUpdatedEvent, CustomerQueryModel>(MemberList.Destination);
        CreateMap<CustomerDeletedEvent, CustomerQueryModel>(MemberList.Destination);
    }
}