using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Shop.Core.Extensions;

public static class JsonExtensions
{
    private static readonly CamelCaseNamingStrategy CamelCaseNaming = new();
    private static readonly StringEnumConverter StringEnumConverter = new(CamelCaseNaming);
    private static readonly PrivateSetterContractResolver ContractResolver = new() { NamingStrategy = CamelCaseNaming };
    private static readonly JsonSerializerSettings DefaultJsonSettings = new JsonSerializerSettings().Configure();

    /// <summary>
    /// Converts a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type T.</returns>
    public static T FromJson<T>(this string value) =>
        value != null ? JsonConvert.DeserializeObject<T>(value, DefaultJsonSettings) : default;

    /// <summary>
    /// Converts an object to JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string ToJson<T>(this T value) =>
        value != null ? JsonConvert.SerializeObject(value, typeof(T), DefaultJsonSettings) : default;

    /// <summary>
    /// Configures the JsonSerializerSettings instance.
    /// </summary>
    /// <param name="jsonSettings">The JsonSerializerSettings instance to configure.</param>
    /// <returns>The configured JsonSerializerSettings instance.</returns>
    public static JsonSerializerSettings Configure(this JsonSerializerSettings jsonSettings)
    {
        // Ignore circular references
        jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        // Disable preserving references
        jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

        // Ignore null values during serialization
        jsonSettings.NullValueHandling = NullValueHandling.Ignore;

        // Disable formatting of the JSON output
        jsonSettings.Formatting = Formatting.None;

        // Set the contract resolver for custom serialization behavior
        jsonSettings.ContractResolver = ContractResolver;

        // Add a converter for string enum values
        jsonSettings.Converters.Add(StringEnumConverter);

        return jsonSettings;
    }
}