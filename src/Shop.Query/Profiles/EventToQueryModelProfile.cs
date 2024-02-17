using AutoMapper;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Domain.Entities.ProductAggregate.Events;
using Shop.Query.QueriesModel;

namespace Shop.Query.Profiles;

public class EventToQueryModelProfile : Profile
{
    public EventToQueryModelProfile()
    {
        CreateMap<CustomerCreatedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateCustomerQueryModel(@event));

        CreateMap<CustomerUpdatedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateCustomerQueryModel(@event));

        CreateMap<CustomerDeletedEvent, CustomerQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateCustomerQueryModel(@event));

        CreateMap<ProductCreatedEvent, ProductQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateProductQueryModel(@event));

        CreateMap<ProductUpdatedEvent, ProductQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateProductQueryModel(@event));

        CreateMap<ProductDeletedEvent, ProductQueryModel>(MemberList.Destination)
            .ConstructUsing(@event => CreateProductQueryModel(@event));
    }

    public override string ProfileName => nameof(EventToQueryModelProfile);

    private static CustomerQueryModel CreateCustomerQueryModel<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent =>
        new(@event.Id, @event.FirstName, @event.LastName, @event.Gender.ToString(), @event.Email, @event.DateOfBirth);

    private static ProductQueryModel CreateProductQueryModel<TEvent>(TEvent @event) where TEvent : ProductBaseEvent =>
        new(@event.Id, @event.Name, @event.Description, @event.Price);
}
