using MongoDB.Bson.Serialization;
using Shop.Core.Events;

namespace Shop.Infrastructure.Data.Events.Mappings;

public static class StoredEventMap
{
    public static void Configure()
    {
        BsonClassMap.TryRegisterClassMap<StoredEvent>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);

            map.MapIdMember(storedEvent => storedEvent.Id);

            map.MapMember(storedEvent => storedEvent.Type)
                .SetIsRequired(true);

            map.MapMember(storedEvent => storedEvent.Data)
                .SetIsRequired(true);
        });
    }
}