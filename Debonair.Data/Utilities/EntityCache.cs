using System.Collections.Generic;
using System.Reflection;

namespace Debonair.Utilities
{
    public static class EntityCache
    {
        private static readonly CacheProvider PropertyInfoCache = new CacheProvider();

        public static IEnumerable<PropertyInfo> GetPropertyInfo(object obj)
        {
            var type = obj.GetType();
            var key = type.FullName;

            if (!PropertyInfoCache.TryGet(key, out IEnumerable<PropertyInfo> value))
            {
                value = type.GetProperties();
                PropertyInfoCache.Set(key, value);
            }

            return value;
        }

    }
}
