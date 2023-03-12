using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shop.Core.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAllTypesOf<TInterface>(this Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => typeof(TInterface).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsInterface)
            .ToList();
    }

    public static IEnumerable<TInterface> GetAllInstacesOf<TInterface>(this Assembly assembly)
    {
        foreach (var impl in assembly.GetAllTypesOf<TInterface>())
            yield return (TInterface)Activator.CreateInstance(impl);
    }
}