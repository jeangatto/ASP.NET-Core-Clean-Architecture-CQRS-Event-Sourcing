using AutoMapper;
using Shop.Domain.Entities.Customer.Events;
using Shop.Domain.QueriesModel;

namespace Shop.Domain.Profiles;

public class EventToQueryModelProfile : Profile
{
    public EventToQueryModelProfile()
    {
        CreateMap<CustomerCreatedEvent, CustomerQueryModel>();
    }
}