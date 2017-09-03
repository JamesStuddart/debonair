using System;
using System.Runtime.Caching;

namespace Debonair.Utilities
{
    /// <summary>
    /// Global caching provider
    /// </summary>
    public class CacheProvider
    {
        private static ObjectCache Cache { get { return MemoryCache.Default; } }

        /// <summary>
        /// Get Item from cache
        /// </summary>
        /// <typeparam name="T">Any object type you wish to store</typeparam>
        /// <param name="key">The key used to identify the value</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// Put an object into the cache
        /// </summary>
        /// <typeparam name="T">Any object type you wish to store</typeparam>
        /// <param name="key">The key used to identify the value</param>
        /// <param name="data">The object you want to store</param>
        /// <param name="cacheTime">The hours you wish to cache the object, default is 24 hours</param>
        public void Set<T>(string key, T data, int cacheTime = 24)
        {
            Cache.Add(new CacheItem(key, data), new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromHours(cacheTime) });
        }


        /// <summary>
        /// Check item exists in cache
        /// </summary>
        /// <param name="key">The key used to identify the value</param>
        public bool IsSet(string key)
        {
            return Cache.Contains(key);
        }

        /// <summary>
        /// Remove specific item from cache
        /// </summary>
        /// <param name="key">The key used to identify the value</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Remove ALL items from the cache
        /// </summary>
        public void Empty()
        {
            foreach (var item in Cache)
            {
                Cache.Remove(item.Key);
            }
        }

    }
}
