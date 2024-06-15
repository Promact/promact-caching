using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Promact.Core.Caching;


namespace Promact.Caching.Test
{
    [TestClass]
    public class RedisCacheTests
    {
        private IDistributedCache _distributedCache;
        private ICachingService _cacheService;
        private string[] _keys = new string[] { "TestKey1", "TestKey2", "TestKey3" };
        [TestInitialize]
        public void Setup()
        {
            _distributedCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = "localhost:6379"
            });

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

        [TestCleanup]
        public void CleanUp()
        {
            _keys.ToList().ForEach(key => _cacheService.Remove(key));
        }
    }

    internal class TestData
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
