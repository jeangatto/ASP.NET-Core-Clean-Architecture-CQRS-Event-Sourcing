using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Shop.Core.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions().Configure();

    /// <summary>
    /// Converts a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type T.</returns>
    public static T FromJson<T>(this string value) =>
        value != null ? JsonSerializer.Deserialize<T>(value, JsonOptions) : default;

    /// <summary>
    /// Converts an object to JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string ToJson<T>(this T value) =>
        value != null ? JsonSerializer.Serialize(value, JsonOptions) : default;

    /// <summary>
    /// Configures the JsonSerializerSettings instance.
    /// </summary>
    /// <param name="jsonSettings">The JsonSerializerSettings instance to configure.</param>
    /// <returns>The configured JsonSerializerSettings instance.</returns>
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

internal sealed class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object
            && jsonTypeInfo.CreateObject is null
            && jsonTypeInfo.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
        {
            jsonTypeInfo.CreateObject = () => Activator.CreateInstance(jsonTypeInfo.Type, true);
        }

        return jsonTypeInfo;
    }
}