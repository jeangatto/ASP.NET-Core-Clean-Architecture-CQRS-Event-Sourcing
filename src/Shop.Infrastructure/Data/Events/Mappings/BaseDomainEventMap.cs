using MongoDB.Bson.Serialization;
using Shop.Core.Abstractions;

namespace Shop.Infrastructure.Data.Events.Mappings;

public static class BaseDomainEventMap
{
    public static void Configure()
    {
        BsonClassMap.TryRegisterClassMap<BaseDomainEvent>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIsRootClass(true);

            map.MapMember(storedEvent => storedEvent.OccurredOn)
                .SetIsRequired(true)
                .SetElementName("CreatedAt");
        });
    }
}