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
            IEnumerable<Type> implementingTypes = GetTypes<T>();

            FieldInfo[] fields = typeof(S).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }

                if (field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    continue;
                }

                Type fieldTypeArgument = field.FieldType.GetGenericArguments()[0];

                foreach (Type type in implementingTypes)
                {
                    if (fieldTypeArgument != type)
                    {
                        continue;
                    }

                    Type listType = typeof(List<>).MakeGenericType(type);
                    object listInstance = Activator.CreateInstance(listType);

                    foreach (var item in listWithDifferentTypes)
                    {
                        if (item.GetType() == type)
                        {
                            AddToList(listInstance, item);
                        }
                    }

                    field.SetValue(instanceWithFields, listInstance);
                }
            }
        }

        public static List<T> MergeFieldListsIntoOneImplementingType<T, S>(S instanceWithFields)
        {
            List<T> result = new List<T>();
            IEnumerable<Type> implementingTypes = GetTypes<T>();

            FieldInfo[] fields = typeof(S).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericType)
                {
                    continue;
                }

                if (field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    continue;
                }

                Type fieldTypeArgument = field.FieldType.GetGenericArguments()[0];
                foreach (Type type in implementingTypes)
                {
                    if (fieldTypeArgument != type)
                    {
                        continue;
                    }

                    result.AddRange(((IEnumerable<T>)field.GetValue(instanceWithFields)));
                }
            }

            return result;
        }

        private static void AddToList(object list, object item)
        {
            Type listType = list.GetType();
            if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
            {
                MethodInfo addMethod = listType.GetMethod("Add");
                if (addMethod != null)
                {
                    addMethod.Invoke(list, new[] { item });
                }
            }
        }
    }
}
