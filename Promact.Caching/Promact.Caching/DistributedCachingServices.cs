using Microsoft.Extensions.Caching.Distributed;
using Promact.Core.Caching;
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
        public T? Get<T>(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var data = _distributedCache.GetString(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public void Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _distributedCache.Remove(key);            
        }

        public void Set<T>(string key, T value)
        {
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(value == null) throw new ArgumentNullException(nameof(value));
            var data = JsonSerializer.Serialize(value);
            _distributedCache.SetString(key, data);
        }
    }
}
