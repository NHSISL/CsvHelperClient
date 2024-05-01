using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CsvHelper.Configuration;

namespace CsvHelperClient.Models.Foundations.CsvHelpers
{
    internal class CustomMap<T> : ClassMap<T>
    {
        public CustomMap(Dictionary<string, int> fieldMappings)
        {
            foreach (var mapping in fieldMappings)
            {
                try
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, mapping.Key);
                    var funcType = typeof(Func<,>).MakeGenericType(typeof(T), property.Type);
                    var lambda = Expression.Lambda(funcType, property, parameter);
                    var mapMethods = typeof(ClassMap<T>).GetMethods().Where(m => m.Name == "Map" && m.IsGenericMethod);

                    var mapMethod = mapMethods.First(m => m.GetParameters().First()
                        .ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

                    mapMethod = mapMethod.MakeGenericMethod(property.Type);
                    var memberMap = (MemberMap)mapMethod.Invoke(this, new object[] { lambda, true });
                    memberMap.Index(mapping.Value);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
