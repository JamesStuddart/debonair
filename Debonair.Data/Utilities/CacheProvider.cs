using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Debonair.Utilities
{
    /// <summary>
    /// Global caching provider
    /// </summary>
    public class CacheProvider
    {
        private readonly IMemoryCache _cache;

        public CacheProvider()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Get Item from cache
        /// </summary>
        /// <typeparam name="T">Any object type you wish to store</typeparam>
        /// <param name="key">The key used to identify the value</param>
        /// <returns></returns>
        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
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
            _cache.Set(key, data, DateTime.Now + TimeSpan.FromHours(cacheTime));
        }

        /// <summary>
        /// Remove specific item from cache
        /// </summary>
        /// <param name="key">The key used to identify the value</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Remove ALL items from the cache
        /// </summary>
        public void Empty()
        {
            foreach (var key in _cache.Get<List<string>>(string.Empty))
            {
                _cache.Remove(key);
            }
        }

    }
}
