using System;
using System.Linq;

namespace Shop.Core.Extensions;

public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this object @object)
        => @object.GetType().GetGenericTypeName();

    private static string GetGenericTypeName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            return type.Name;
        }
    }
}