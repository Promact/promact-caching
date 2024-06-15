using ArgumentValidator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Promact.Core.Caching;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json;

namespace Promact.Caching
{
    public class DistributedCachingServices : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        public DistributedCachingServices(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public T Get<T>(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var data = _distributedCache.GetString(key);
#pragma warning disable CS8603 // Possible null reference return.
            return string.IsNullOrEmpty(data) ? default(T) : JsonSerializer.Deserialize<T>(data);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _distributedCache.Remove(key);
        }

        public void Set<T>(string key, T value)
        {
            Throw.IfNull(key, nameof(key));
            Throw.IfNull(value, nameof(value));
            var data = JsonSerializer.Serialize(value);
            _distributedCache.SetString(key, data);
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        public HashSet<string> GetAllKeys()
        {
            //Check if the Caching provider is Redis then cast it to RedisCache and get the keys
            if (_distributedCache is RedisCache)
            {
                var redisCache = _distributedCache as RedisCache;

                IDatabase db = redisCache.GetType().GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(redisCache) as IDatabase;
                var keysResult = ((RedisValue[])db.Execute("KEYS", "*"));
                return new HashSet<string>(keysResult.Select(k => k.ToString()));

            }
            //Check if the Caching provider is InMemory then cast it and return all the keys
            else if (_distributedCache is MemoryDistributedCache)
            {
                var memoryCache = _distributedCache as MemoryDistributedCache;
                var cache = memoryCache.GetType().GetField("_memCache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(memoryCache) as MemoryCache;
                dynamic _coherentState = cache.GetType().GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(cache);
                var keys = new HashSet<string>();
                foreach (KeyValuePair<object, dynamic> pairs in _coherentState._entries)
                {
                    keys.Add(Convert.ToString(pairs.Key));
                }           
                return keys;
            }
            else
            {
                throw new NotSupportedException("This method is not supported for the current caching provider");
            }
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
    }
}