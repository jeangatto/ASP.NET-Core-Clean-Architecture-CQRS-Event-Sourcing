using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Mappings;

public class CustomerMap : IReadDbMapping
{
    public void Configure()
    {
        BsonClassMap.TryRegisterClassMap<CustomerQueryModel>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);

            map.MapMember(customer => customer.FirstName)
                .SetIsRequired(true);

            map.MapMember(customer => customer.LastName)
                .SetIsRequired(true);

            map.MapMember(customer => customer.Gender)
                .SetIsRequired(true);

            map.MapMember(customer => customer.Email)
                .SetIsRequired(true);

            map.MapMember(customer => customer.DateOfBirth)
                .SetIsRequired(true)
                .SetSerializer(new DateTimeSerializer(true));

            // Ignore
            map.UnmapMember(m => m.FullName);
        });
    }
}