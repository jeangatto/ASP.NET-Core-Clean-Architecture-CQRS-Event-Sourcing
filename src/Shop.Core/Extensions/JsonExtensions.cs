using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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

internal sealed class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
{
    /// <summary>
    /// Gets the JSON type information for the specified type, with support for creating objects with private constructors.
    /// </summary>
    /// <param name="type">The type to get the JSON type information for.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use for serialization.</param>
    /// <returns>The JSON type information for the specified type.</returns>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        // Check if the type is an object, has no public constructor, and CreateObject is not already set
        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object
            && jsonTypeInfo.CreateObject is null
            && jsonTypeInfo.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
        {
            // Set CreateObject to a lambda expression that creates an instance using a private constructor
            jsonTypeInfo.CreateObject = () => Activator.CreateInstance(jsonTypeInfo.Type, true);
        }

        return jsonTypeInfo;
    }
}