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
    private static readonly StringEnumConverter StringEnumConverter = new(NamingStrategy);
    private static readonly IContractResolver ContractResolver = new PrivateSetterContractResolver { NamingStrategy = NamingStrategy };
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings().Configure();

    /// <summary>
    /// Desserializa o JSON para o tipo especificado.
    /// </summary>
    /// <typeparam name="T">O tipo de objeto para o qual desserializar.</typeparam>
    /// <param name="value">O objeto a ser desserializado.</param>
    /// <returns>O objeto desserializado da string JSON.</returns>
    public static T FromJson<T>(this string value)
        => JsonConvert.DeserializeObject<T>(value, JsonSettings);

    /// <summary>
    /// Serializa o objeto especificado em uma string JSON.
    /// </summary>
    /// <param name="value">O objeto a ser serializado.</param>
    /// <returns>Uma representação de string JSON do objeto.</returns>
    public static string ToJson<T>(this T value)
        => JsonConvert.SerializeObject(value, JsonSettings);

    public static JsonSerializerSettings Configure(this JsonSerializerSettings settings)
    {
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.Formatting = Formatting.None;
        settings.ContractResolver = ContractResolver;
        settings.Converters.Add(StringEnumConverter);
        return settings;
    }
}