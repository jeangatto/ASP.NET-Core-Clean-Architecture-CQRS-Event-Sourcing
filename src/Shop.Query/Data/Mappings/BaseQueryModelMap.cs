using MongoDB.Bson.Serialization;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Mappings;

public class BaseQueryModelMap : IReadDbMapping
{
    public void Configure()
    {
        // TryRegisterClassMap: Registers a class map if it is not already registered.
        BsonClassMap.TryRegisterClassMap<BaseQueryModel>(classMap =>
        {
            classMap.AutoMap();
            classMap.SetIgnoreExtraElements(true);
            classMap.SetIsRootClass(true);

            classMap.MapIdMember(queryModel => queryModel.Id)
                .SetIsRequired(true);
        });
    }
}