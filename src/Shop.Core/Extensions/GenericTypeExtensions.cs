using System.Linq;

namespace Shop.Core.Extensions;

public static class GenericTypeExtensions
{
    /// <summary>
    /// Returns the name of the generic type of the object.
    /// </summary>
    /// <param name="object">The object to get the generic type name from.</param>
    /// <returns>The name of the generic type.</returns>
    public static string GetGenericTypeName(this object @object)
    {
        var type = @object.GetType();

        // Check if the type is not generic
        if (!type.IsGenericType)
            return type.Name;

        // Get the names of the generic arguments and join them with commas
        var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());

        // Remove the backtick and append the generic arguments to the type name
        return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
    }
}