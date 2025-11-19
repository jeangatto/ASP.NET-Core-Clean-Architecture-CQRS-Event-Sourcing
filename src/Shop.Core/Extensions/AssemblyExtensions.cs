using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shop.Core.Extensions;

public static class AssemblyExtensions
{
    extension<TInterface>(Assembly assembly)
    {
        /// <summary>
        /// Retrieves all types in the specified assembly that implement or inherit from a given interface or base class.
        /// </summary>
        /// <returns>An enumerable collection of types that implement or inherit from the specified interface or base class.</returns>
        public IEnumerable<Type> GetAllTypesOf()
        {
            var isAssignableToTInterface = typeof(TInterface).IsAssignableFrom;

            return assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && isAssignableToTInterface(type));
        }
    }
}