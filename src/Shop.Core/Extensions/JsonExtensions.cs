using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shop.Core.JsonResolvers;

namespace Shop.Core.Extensions;

public static class JsonExtensions
{
    private static readonly Lazy<JsonSerializerOptions> LazyOptions =
        new(() => new JsonSerializerOptions().Configure(), isThreadSafe: true);

    /// <summary>
    /// Converts a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type T.</returns>
    public static T FromJson<T>(this string value) =>
        value != null ? JsonSerializer.Deserialize<T>(value, LazyOptions.Value) : default;

    /// <summary>
    /// Converts an object to JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string ToJson<T>(this T value) =>
        !value.IsDefault() ? JsonSerializer.Serialize(value, LazyOptions.Value) : default;

    /// <summary>
    /// Configures the JsonSerializerOptions instance.
    /// </summary>
    /// <param name="jsonSettings">The JsonSerializerOptions instance to configure.</param>
    /// <returns>The configured JsonSerializerOptions instance.</returns>
    public static JsonSerializerOptions Configure(this JsonSerializerOptions jsonSettings)
    {
        jsonSettings.WriteIndented = false;
        jsonSettings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSettings.ReadCommentHandling = JsonCommentHandling.Skip;
        jsonSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSettings.TypeInfoResolver = new PrivateConstructorContractResolver();
        jsonSettings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return jsonSettings;
    }
}