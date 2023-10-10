using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shop.Core.Extensions;

public static class AssemblyExtensions
{
    /// <summary>
    /// Retrieves all types in the specified assembly that implement or inherit from a given interface or base class.
    /// </summary>
    /// <typeparam name="TInterface">The interface or base class type.</typeparam>
    /// <param name="assembly">The assembly to search in.</param>
    /// <returns>An enumerable collection of types that implement or inherit from the specified interface or base class.</returns>
    public static IEnumerable<Type> GetAllTypesOf<TInterface>(this Assembly assembly)
    {
        var isAssignableToTInterface = typeof(TInterface).IsAssignableFrom;
        return assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && isAssignableToTInterface(type))
            .ToList();
    }
}