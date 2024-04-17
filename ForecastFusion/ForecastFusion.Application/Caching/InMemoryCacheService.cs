using ForecastFusion.Application.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ForecastFusion.Api.Caching
{
    public class InMemoryCacheService: ICacheService
    {
        private const int SLIDING_EXPIRATION_IN_SECONDS = 60;
        private const int ABSOLUTE_EXPIRATION_IN_SECONDS = 86400; //24 hours
        private readonly IMemoryCache _cache;
        private MemoryCacheEntryOptions memoryCacheEntryOptions;

        public InMemoryCacheService(IMemoryCache memoryCache) 
        {
            _cache = memoryCache;
            memoryCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(SLIDING_EXPIRATION_IN_SECONDS))
                                                                   .SetAbsoluteExpiration(TimeSpan.FromSeconds(ABSOLUTE_EXPIRATION_IN_SECONDS));
        }

        public object GetValue(string key)
        {
            object result;
            if (_cache.TryGetValue(key, out result!))
            {
                return result;
            }
            return null!;
        }

        public void RemoveValue(string key)
        {
            _cache.Remove(key);
        }

        public void SetValue(string key, object value)
        {
            _cache.Set(key, value, memoryCacheEntryOptions);
        }
    }
}
