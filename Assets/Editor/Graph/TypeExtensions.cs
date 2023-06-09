using System;
using System.Collections.Generic;
using System.Linq;

namespace Chocolate4.Edit.Graph
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetTypes<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(type => typeof(T).IsAssignableFrom(type))
                            .Where(type => !type.IsInterface && !type.IsAbstract);
        }
    }
}
