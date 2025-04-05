using System.Text.Json.Serialization;

namespace Shop.Domain.Entities.CustomerAggregate;

[JsonConverter(typeof(JsonStringEnumConverter<EGender>))]
public enum EGender
{
    None = 0,
    Male = 1,
    Female = 2
}