using MongoDB.Bson.Serialization;
using Shop.Core.Events;

namespace Shop.Infrastructure.Data.Mappings.ReadOnly;

public class EventStoreMap : IReadDbMapping
{
    public void Configure()
    {
        BsonClassMap.TryRegisterClassMap<EventStore>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);

            map.MapIdMember(eventStore => eventStore.Id);
            map.MapMember(eventStore => eventStore.Type).SetIsRequired(true);
            map.MapMember(eventStore => eventStore.Data).SetIsRequired(true);
        });
    }
}