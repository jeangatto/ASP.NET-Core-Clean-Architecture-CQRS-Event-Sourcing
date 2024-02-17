using MongoDB.Bson.Serialization;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Mappings;

public class ProductMap : IReadDbMapping
{
    public void Configure()
    {
        BsonClassMap.TryRegisterClassMap<ProductQueryModel>(classMap =>
        {
            classMap.AutoMap();
            classMap.SetIgnoreExtraElements(true);

            classMap.MapMember(product => product.Id)
                .SetIsRequired(true);

            classMap.MapMember(product => product.Name)
                .SetIsRequired(true);

            classMap.MapMember(product => product.Description)
                .SetIsRequired(true);

            classMap.MapMember(product => product.Price)
                .SetIsRequired(true);
        });
    }
}
