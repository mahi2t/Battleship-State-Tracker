using Microsoft.Extensions.Caching.Memory;
using System;

namespace Battleship.Tracker.Features.StateTracker
{
    public interface ICacheService
    {
        Tuple<bool, T> GetDataFromCache<T>(string cacheKey);

        void CreateOrUpdateCache(string cacheKey, object value);
    }

    /// <summary>
    /// CacheService
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Creates a new or updates and existing cache.
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="value">value</param>
        public void CreateOrUpdateCache(string cacheKey, object value)
        {
            _ = memoryCache.Set(cacheKey, value);
        }

        /// <summary>
        /// Gets existing cache
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>Tuple<bool, T></returns>
        public Tuple<bool, T> GetDataFromCache<T>(string cacheKey)
        {
            var isExists = memoryCache.TryGetValue<T>(cacheKey, out T name);
            return Tuple.Create(isExists, name);
        }
    }
}
