using Microsoft.Extensions.Caching.Distributed;
using Promact.Core.Caching;

namespace Promact.Caching.Test
{
    [TestClass]
    public class NotSupportedProviderTest
    {        
        private ICachingService _cacheService;        
        [TestInitialize]
        public void Setup()
        {            
            var _distributedCache = new FakeCachingProvider();
            _cacheService = new DistributedCachingServices(_distributedCache);
        }        

        [TestMethod]
        public void GetAllKeyNotSupportedTest()
        {
            Assert.ThrowsException<NotSupportedException>(() => _cacheService.GetAllKeys());             
        }
                
    }

    internal class FakeCachingProvider : IDistributedCache
    {
        public byte[] Get(string key)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
