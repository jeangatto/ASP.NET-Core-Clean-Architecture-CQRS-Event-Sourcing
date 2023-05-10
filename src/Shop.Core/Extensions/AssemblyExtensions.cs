using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shop.Core.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAllTypesOf<TInterface>(this Assembly assembly)
    {
        var isAssignableToTInterface = typeof(TInterface).IsAssignableFrom;
        return assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && isAssignableToTInterface(t))
            .ToList();
    }

    public static IEnumerable<TInterface> GetAllInstacesOf<TInterface>(this Assembly assembly) =>
        assembly.GetAllTypesOf<TInterface>().Select(impl => (TInterface)Activator.CreateInstance(impl));
}