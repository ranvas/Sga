using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    public static class DbDataReaderExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> _materializers = new ConcurrentDictionary<Type, object>();

        public static IEnumerable<T> Translate<T>(this DbDataReader reader) where T : new()
        {
            var materializer = (Func<IDataRecord, T>)_materializers.GetOrAdd(typeof(T), (Func<IDataRecord, T>)Materializer.Materialize<T>);
            return Translate(reader, materializer);
        }

        public static IEnumerable<T> TranslateValue<T>(this DbDataReader reader)
        {
            var materializer = (Func<IDataRecord, T>)_materializers.GetOrAdd(typeof(T), (Func<IDataRecord, T>)Materializer.MaterializeValue<T>);
            return Translate(reader, materializer);
        }

        public static IEnumerable<T> Translate<T>(this DbDataReader reader, Func<IDataRecord, T> objectMaterializer)
        {
            var results = new List<T>();
            while (reader.Read())
            {
                var record = (IDataRecord)reader;
                var obj = objectMaterializer(record);
                results.Add(obj);
            }

            return results;
        }
        /// <summary>
        /// From here: https://dzone.com/articles/aspnet-core-with-entity-framework-core-returning-m
        /// </summary>
        private class Materializer
        {
            public static T Materialize<T>(IDataRecord record) where T : new()
            {
                var t = new T();
                foreach (var prop in typeof(T).GetProperties())
                {
                    // 1). If entity reference, bypass it.
                    if (prop.PropertyType.Namespace == typeof(T).Namespace)
                    {
                        continue;
                    }

                    // 2). If collection, bypass it.
                    if (prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        continue;
                    }

                    // 3). If property is NotMapped, bypass it.
                    if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                    {
                        continue;
                    }

                    var dbValue = record[prop.Name];
                    if (dbValue is DBNull) continue;

                    if (prop.PropertyType.IsConstructedGenericType &&
                        prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var baseType = prop.PropertyType.GetGenericArguments()[0];
                        var baseValue = Convert.ChangeType(dbValue, baseType);
                        var value = Activator.CreateInstance(prop.PropertyType, baseValue);
                        prop.SetValue(t, value);
                    }
                    else
                    {
                        var value = Convert.ChangeType(dbValue, prop.PropertyType);
                        prop.SetValue(t, value);
                    }
                }

                return t;
            }

            public static T MaterializeValue<T>(IDataRecord record)
            {
                return (T)record.GetValue(0);
            }
        }
    }
}
