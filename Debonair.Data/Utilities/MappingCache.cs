using System.Reflection;
using Debonair.FluentApi;

namespace Debonair.Utilities
{
    public static class MappingCache
    {
        private static readonly CacheProvider PropertyMappingCache = new CacheProvider();
        private static readonly CacheProvider EntityMappingCache = new CacheProvider();

        public static IPropertyMapping GetPropertyMapping<TEntity>(PropertyInfo prop) where TEntity : class, new()
        {
            var key = prop.GetType().FullName;

            if (!PropertyMappingCache.TryGet(key, out IPropertyMapping value))
            {
                value = GetMappingForEntity<TEntity>().GetMappingForType(prop);
                PropertyMappingCache.Set(key, value);
            }

            return value;
        }

        public static IEntityMapping<TEntity> GetMappingForEntity<TEntity>() where TEntity : class, new()
        {
            var key = typeof(TEntity).FullName;

            if (!EntityMappingCache.TryGet(key, out IEntityMapping<TEntity> value))
            {
                value = EntityMappingEngine.GetMappingForEntity<TEntity>();
                EntityMappingCache.Set(key, value);
            }

            return value;
        }
    }
}