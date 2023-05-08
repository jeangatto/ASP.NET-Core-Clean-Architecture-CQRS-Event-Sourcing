using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Shop.Core.Extensions;

/// <summary>
/// Extensões para a utilização do JSON.
/// </summary>
public static class JsonExtensions
{
    private static readonly CamelCaseNamingStrategy NamingStrategy = new();
    private static readonly StringEnumConverter EnumConverter = new(NamingStrategy);
    private static readonly PrivateSetterContractResolver ContractResolver = new() { NamingStrategy = NamingStrategy };
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings().Configure();

    public static JsonSerializerSettings Configure(this JsonSerializerSettings jsonSettings)
    {
        jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
        jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        jsonSettings.Formatting = Formatting.None;
        jsonSettings.ContractResolver = ContractResolver;
        jsonSettings.Converters.Add(EnumConverter);
        return jsonSettings;
    }

    /// <summary>
    /// Desserializa o JSON para o tipo especificado.
    /// </summary>
    /// <typeparam name="T">O tipo de objeto para o qual desserializar.</typeparam>
    /// <param name="value">O objeto a ser desserializado.</param>
    /// <returns>O objeto desserializado da string JSON.</returns>
    public static T FromJson<T>(this string value) =>
        value != null ? JsonConvert.DeserializeObject<T>(value, JsonSettings) : default;

    /// <summary>
    /// Serializa o objeto especificado em uma string JSON.
    /// </summary>
    /// <param name="value">O objeto a ser serializado.</param>
    /// <returns>Uma representação de string JSON do objeto.</returns>
    public static string ToJson<T>(this T value) =>
        value != null ? JsonConvert.SerializeObject(value, JsonSettings) : default;
}