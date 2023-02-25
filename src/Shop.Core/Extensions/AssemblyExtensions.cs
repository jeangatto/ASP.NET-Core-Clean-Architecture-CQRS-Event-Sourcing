using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shop.Core.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAllTypesOfInterface<TInterface>(this Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => typeof(TInterface).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsInterface)
            .ToList();
    }

    public static IEnumerable<TInterface> GetAllInstacesOfInterface<TInterface>(this Assembly assembly)
    {
        foreach (var impl in assembly.GetAllTypesOfInterface<TInterface>())
        {
            yield return (TInterface)Activator.CreateInstance(impl);
        }
    }
}