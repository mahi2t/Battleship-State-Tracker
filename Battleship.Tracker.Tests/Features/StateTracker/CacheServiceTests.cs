using Battleship.Tracker.Features.StateTracker;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace Battleship.Tracker.Tests.Features.StateTracker
{
    [TestFixture]
    public class CacheServiceTests
    {
        private Mock<IMemoryCache> memoryCache;
        private ICacheService cacheService;

        [SetUp]
        public void Setup()
        {
            memoryCache = new Mock<IMemoryCache>();
            cacheService = new CacheService(memoryCache.Object);
        }

        [Test]
        public void CreateOrUpdateCache_Should_add_data_to_cache()
        {
            // Arrange
            memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

            // Act
            cacheService.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>());

            // Assert
            memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void GetDataFromCache_Should_return_data_from_cache()
        {
            // Arrange
            var someObject = new object();
            memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out someObject)).Returns(false);

            // Act
            cacheService.GetDataFromCache<bool>(It.IsAny<string>());

            // Assert
            memoryCache.Verify(x => x.TryGetValue(It.IsAny<object>(), out someObject), Times.Once);

        }
    }
}
