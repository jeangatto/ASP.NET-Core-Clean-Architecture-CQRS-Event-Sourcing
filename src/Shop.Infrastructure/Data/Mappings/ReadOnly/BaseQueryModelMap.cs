using MongoDB.Bson.Serialization;
using Shop.Core.Abstractions;

namespace Shop.Infrastructure.Data.Mappings.ReadOnly;

public static class BaseQueryModelMap
{
    public static void Configure()
    {
        BsonClassMap.TryRegisterClassMap<BaseQueryModel>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
            map.SetIsRootClass(true);

            map.MapIdMember(queryModel => queryModel.Id);
        });
    }
}