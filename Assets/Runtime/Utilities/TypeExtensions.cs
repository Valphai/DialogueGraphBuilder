using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chocolate4.Dialogue.Runtime.Utilities
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetTypes<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(type => typeof(T).IsAssignableFrom(type))
                            .Where(type => !type.IsInterface && !type.IsAbstract);
        }

        public static void DistributeListElementsToFieldsOfImplementingTypes<T, S>(List<T> listWithDifferentTypes, S instanceWithFields)
        {
            List<Type> implementingTypes = GetTypes<T>().ToList();
            Type genericListType = typeof(List<>);

            Dictionary<T, Type> elementToType = new Dictionary<T, Type>();

            foreach (var item in listWithDifferentTypes)
            {
                if (!elementToType.ContainsKey(item))
                {
                    elementToType.Add(item, item.GetType());
                }
            }

            FieldInfo[] fields = typeof(S).GetFields();

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }

                Type genericArgument = field.FieldType.GetGenericArguments().First();
                if (genericArgument == null)
                {
                    continue;
                }

                foreach (Type type in implementingTypes)
                {
                    if (genericArgument != type)
                    {
                        continue;
                    }

                    Type listType = genericListType.MakeGenericType(type);
                    object listInstance = Activator.CreateInstance(listType);
                    MethodInfo addMethod = listType.GetMethod("Add");

                    foreach (var item in listWithDifferentTypes)
                    {
                        Type itemType = elementToType[item];
                        if (itemType == type)
                        {
                            addMethod.Invoke(listInstance, new[] { (object)item });
                        }
                    }

                    field.SetValue(instanceWithFields, listInstance);
                }
            }
        }

        public static List<T> MergeFieldListsIntoOneImplementingType<T, S>(S instanceWithFields)
        {
            List<T> result = new List<T>();
            List<Type> implementingTypes = GetTypes<T>().ToList();

            FieldInfo[] fields = typeof(S).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }

                Type genericArgument = field.FieldType.GetGenericArguments().First();
                if (genericArgument == null)
                {
                    continue;
                }

                foreach (Type type in implementingTypes)
                {
                    if (genericArgument != type)
                    {
                        continue;
                    }

                    result.AddRange((IEnumerable<T>)field.GetValue(instanceWithFields));
                }
            }

            return result;
        }

        public static List<T> GetListOfTypeFrom<T, S>(S instanceWithFields)
        {
            FieldInfo[] fields = typeof(S).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }

                Type genericArgument = field.FieldType.GetGenericArguments().First();
                if (genericArgument == null)
                {
                    continue;
                }

                return (List<T>)field.GetValue(instanceWithFields);
            }

            return null;
        }
    }
}
