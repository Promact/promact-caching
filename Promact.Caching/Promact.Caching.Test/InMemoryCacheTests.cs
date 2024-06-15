using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Promact.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Caching.Test
{    
    [TestClass]
    public class InMemoryCacheTests
    {
        private IDistributedCache _distributedCache;
        private ICachingService _cacheService;
        private string[] _keys = new string[] { "TestKey1", "TestKey2", "TestKey3" };
        [TestInitialize]
        public void Setup()
        {
            IOptions<MemoryDistributedCacheOptions> options = Options.Create(new MemoryDistributedCacheOptions());
            _distributedCache = new MemoryDistributedCache(options);
            _cacheService = new DistributedCachingServices(_distributedCache);
        }

        [TestMethod]
        public void AddDataSuccessTest()
        {
            var data = new TestData() { Name = "Test", Age = 20 };
            _cacheService.Set("TestKey1", data);

            var result = _cacheService.Get<TestData>("TestKey1");
            Assert.IsNotNull(result);
            Assert.AreEqual(data.Name, result.Name);
            Assert.AreEqual(data.Age, result.Age);
        }

        [TestMethod]
        public void GetAllKeysSuccessTest()
        {
            var data = new TestData() { Name = "Test", Age = 20 };
            _cacheService.Set("TestKey1", data);
            _cacheService.Set("TestKey2", data);
            _cacheService.Set("TestKey3", data);

            var keys = _cacheService.GetAllKeys();
            Assert.IsNotNull(keys);
            Assert.AreEqual(3, keys.Count);
        }

        [TestMethod]
        public void GetAllDataSuccessTest()
        {
            var data = new TestData() { Name = "Test", Age = 20 };
            _cacheService.Set("TestKey1", data);
            _cacheService.Set("TestKey2", data);
            _cacheService.Set("TestKey3", data);

            var result = _cacheService.GetAll();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void NullDataTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _cacheService.Set<TestData>("TestKey1", null));
            Assert.ThrowsException<ArgumentNullException>(() => _cacheService.Set<TestData>(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => _cacheService.Get<TestData>(null));
            Assert.ThrowsException<ArgumentNullException>(() => _cacheService.Remove(null));
        }

        [TestMethod]
        public void AddDataRetriveNullTest()
        {
            var data = new TestData() { Name = "Test", Age = 20 };
            _cacheService.Set("TestKey1", data);

            var result = _cacheService.Get<TestData>("TestKey2");
            Assert.IsNull(result);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _keys.ToList().ForEach(key => _cacheService.Remove(key));
        }
    }
}
