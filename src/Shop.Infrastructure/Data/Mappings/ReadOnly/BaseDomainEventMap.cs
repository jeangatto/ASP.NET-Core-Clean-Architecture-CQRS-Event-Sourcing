using MongoDB.Bson.Serialization;
using Shop.Core.Abstractions;

namespace Shop.Infrastructure.Data.Mappings.ReadOnly;

public class BaseDomainEventMap : IReadDbMapping
{
    public void Configure()
    {
        BsonClassMap.TryRegisterClassMap<BaseDomainEvent>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIsRootClass(true);

            map.MapMember(domainEvent => domainEvent.OccurredOn)
                .SetIsRequired(true)
                .SetElementName("CreatedAt");
        });
    }
}