using AutoMapper;
using Shop.Domain.Entities.Customer;
using Shop.Domain.QueriesModel;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Profiles;

public class QueryModelToEntityProfile : Profile
{
    public QueryModelToEntityProfile()
    {
        // Desabilitando o mapeando via construtor.
        // A classe só deverá ser inicializada no construtor manualmente (via dev).
        DisableConstructorMapping();

        // Mapeando propriedades com "setters" privados.
        ShouldMapProperty = _ => true;

        CreateMap<CustomerQueryModel, Customer>(MemberList.Destination)
            .ForMember(dest => dest.Email, cfg => cfg.MapFrom(src => new Email(src.Email)))
            .ForMember(dest => dest.DomainEvents, cfg => cfg.Ignore())
            .AfterMap((_, customer) => customer.ClearDomainEvents());
    }
}