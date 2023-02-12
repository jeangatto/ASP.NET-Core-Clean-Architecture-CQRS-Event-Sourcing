using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Shop.Domain.QueriesModel;

namespace Shop.Infrastructure.Data.Mappings.ReadOnly;

public class CustomerMap : IReadDbMapping
{
    public void Configure()
    {
        BsonClassMap.TryRegisterClassMap<CustomerQueryModel>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);

            map.MapMember(customer => customer.FirstName).SetIsRequired(true);
            map.MapMember(customer => customer.LastName).SetIsRequired(true);
            map.MapMember(customer => customer.Gender).SetIsRequired(true);
            map.MapMember(customer => customer.Email).SetIsRequired(true);

            map.MapMember(customer => customer.DateOfBirth)
                .SetIsRequired(true)
                .SetSerializer(new DateTimeSerializer(dateOnly: true));
        });
    }
}