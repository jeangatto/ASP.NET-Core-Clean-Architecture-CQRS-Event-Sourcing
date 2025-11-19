using System.Linq;

namespace Shop.Core.Extensions;

public static class GenericTypeExtensions
{
    extension<T>(T value)
    {
        public bool IsDefault() => Equals(value, default(T));
    }

    extension(object @object)
    {
        /// <summary>
        /// Returns the name of the generic type of the object.
        /// </summary>
        /// <returns>The name of the generic type.</returns>
        public string GetGenericTypeName()
        {
            var type = @object.GetType();

            // Check if the type is not generic
            if (!type.IsGenericType)
                return type.Name;

            // Get the names of the generic arguments and join them with commas
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());

            // Remove the backtick and append the generic arguments to the type name
            return $"{type.Name[..type.Name.IndexOf('`')]}<{genericTypes}>";
        }
    }
}