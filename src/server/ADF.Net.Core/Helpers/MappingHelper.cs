using System;
using System.Linq;
using System.Reflection;

namespace ADF.Net.Core.Helpers
{

    public static class MappingHelper
    {

        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            var sourceProperties = typeof(TSource).GetProperties(bindingFlags).Where(propertyInfo => propertyInfo.CanRead).Select(propertyInfo => new
            {
                propertyInfo.Name,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            });

            var targetProperties = target.GetType().GetProperties(bindingFlags).Where(propertyInfo => propertyInfo.CanWrite).Select(propertyInfo => new
            {
                propertyInfo.Name,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            });

            foreach (var property in sourceProperties.Intersect(targetProperties))
            {
                var propertyInfo = source.GetType().GetProperty(property.Name);
                if (propertyInfo == null) continue;
                var value = propertyInfo.GetValue(source, null);
                var propertyInfos = target.GetType().GetProperty(property.Name);
                if (propertyInfos != null) propertyInfos.SetValue(target, value, null);
            }

            return target;

        }

        public static TTarget CreateMapped<TSource, TTarget>(this TSource source) where TTarget : new()
        {

            return source.MapTo(new TTarget());

        }
    }
}
