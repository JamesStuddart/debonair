using System.Collections.Generic;
using System.Reflection;
using Debonair.FluentApi;

namespace Debonair.Utilities
{
    public class EntityCache
    {
        private readonly CacheProvider cache;

        public EntityCache()
        {
            cache = new CacheProvider();
        }

        public IEnumerable<PropertyInfo> GetPropertyInfo(object obj)
        {
            var type = obj.GetType();
            var key = type.FullName;

            if (cache.IsSet(key))
            {
                return cache.Get<IEnumerable<PropertyInfo>>(key);
            }
            else
            {
                var props = type.GetProperties();
                cache.Set(key, props);

                return props;
            }
        }
    }
    public class MappingCache
    {
        private readonly CacheProvider cache;

        public MappingCache()
        {
            cache = new CacheProvider();
        }

        public IPropertyMapping GetMapping<TEntity>(PropertyInfo prop) where TEntity : class, new()
        {
            var type = prop.GetType();
            var key = type.FullName;
            
            if (cache.IsSet(key))
            {
                return cache.Get<IPropertyMapping>(key);
            }
            else
            {
                var mapping = EntityMappingEngine.GetMappingForEntity<TEntity>();
                mapping.GetMappingForType(prop);

                var propMapping = mapping.GetMappingForType(prop);

                cache.Set(key, propMapping);

                return propMapping;
            }
        }
    }
}
