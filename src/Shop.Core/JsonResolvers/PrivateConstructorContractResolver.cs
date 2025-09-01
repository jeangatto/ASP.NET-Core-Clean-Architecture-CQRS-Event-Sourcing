using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Shop.Core.JsonResolvers;

public sealed class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
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