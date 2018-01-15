using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NormalNet
{
    public class Normalizer
    {
        private static bool IsSimple(Type type)
        {
            while (true) {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    // nullable type, check if the nested type is simple.
                    type = type.GenericTypeArguments[0];
                    continue;
                }

                return typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string) || type == typeof(decimal);
            }
        }

        private static Dictionary<string, object> ToNormalizedDictionary(object obj,
            Dictionary<string, Dictionary<string, object>> entitiesByType)
        {
            var dictionary = new Dictionary<string, object>();

            var properties = obj.GetType().GetRuntimeProperties();

            foreach (var property in properties) {
                if (IsSimple(property.PropertyType)) {
                    dictionary[property.Name] = property.GetValue(obj);
                } else if (IsEnumerable(property.PropertyType)) {
                    AddItems(obj, entitiesByType, property, dictionary);
                } else {
                    AddPropertyAsDictionary(obj, entitiesByType, property, dictionary);
                }
            }

            return dictionary;
        }

        private static void AddItems(object obj, Dictionary<string, Dictionary<string, object>> entitiesByType,
            PropertyInfo property, Dictionary<string, object> dictionary)
        {
            var enumerableType = property.PropertyType.GetTypeInfo().ImplementedInterfaces
                .First(IsEnumerable).GetTypeInfo();
            if (enumerableType.IsGenericType) {
                var childType = enumerableType.GenericTypeArguments[0];
                if (!IsSimple(childType)) {
                    EnsureDictionary(entitiesByType, childType.Name);
                    var idProperty = childType.GetRuntimeProperty("Id");
                    var ids = new List<string>();
                    foreach (var item in (IEnumerable) property.GetValue(obj)) {
                        var id = idProperty.GetValue(item);
                        entitiesByType[childType.Name][id.ToString()] =
                            ToNormalizedDictionary(item, entitiesByType);
                        ids.Add(id.ToString());
                    }

                    dictionary[property.Name] = ids;
                }
            }
        }

        private static object AddPropertyAsDictionary(object obj,
            Dictionary<string, Dictionary<string, object>> entitiesByType, PropertyInfo property,
            Dictionary<string, object> dictionary)
        {
            var propertyValue = property.GetValue(obj);
            var idProperty = propertyValue.GetType().GetRuntimeProperty("Id");
            if (idProperty == null) {
                throw new NotImplementedException();
            }

            var id = idProperty.GetValue(propertyValue);
            var propertyTypeName = property.PropertyType.Name;
            EnsureDictionary(entitiesByType, propertyTypeName);
            entitiesByType[propertyTypeName][id.ToString()] =
                ToNormalizedDictionary(propertyValue, entitiesByType);
            dictionary[property.Name + "Id"] = id;
            return id;
        }

        public Dictionary<string, object> Normalize(object obj)
        {
            var type = obj.GetType();

            var properties = type.GetRuntimeProperties();
            var result = new Dictionary<string, object>();

            var entitiesByType = new Dictionary<string, Dictionary<string, object>>();

            foreach (var property in properties) {
                if (IsSimple(property.PropertyType)) {
                    result[property.Name] = property.GetValue(obj);
                } else if (IsEnumerable(property.PropertyType)) {
                    AddItems(obj, entitiesByType, property, result);
                } else {
                    var id = AddPropertyAsDictionary(obj, entitiesByType, property, result);
                }
            }

            result["Entities"] = entitiesByType;

            return result;
        }

        private static bool IsEnumerable(Type type) => typeof(IEnumerable).GetTypeInfo()
            .IsAssignableFrom(type.GetTypeInfo());

        private static void EnsureDictionary(IDictionary<string, Dictionary<string, object>> entitiesByType,
            string propertyTypeName)
        {
            if (!entitiesByType.ContainsKey(propertyTypeName))
                entitiesByType[propertyTypeName] = new Dictionary<string, object>();
        }
    }
}