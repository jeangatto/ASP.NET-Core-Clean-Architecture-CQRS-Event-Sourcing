using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Shop.Domain.Entities.CustomerAggregate;

[JsonConverter(typeof(JsonStringEnumConverter<EGender>))]
public enum EGender
{
    [Description("Não informar")]
    None = 0,

    [Description("Masculino")]
    Male = 1,

    [Description("Feminino")]
    Female = 2
}