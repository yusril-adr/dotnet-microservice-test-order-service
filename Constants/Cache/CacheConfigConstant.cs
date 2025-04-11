using Microsoft.Extensions.Caching.Distributed;

namespace DotNetService.Constants.Cache
{
    public class CacheConstant
    {

        public const string REDIS_CACHE_NAME_PREFIX = "cache";
        
        public static DistributedCacheEntryOptions OPTIONS = new()
        {
            AbsoluteExpiration = new DateTimeOffset(new DateTime(2089, 10, 17, 07, 00, 00), TimeSpan.Zero),
            SlidingExpiration = TimeSpan.FromSeconds(30)
        };

    }
}