
using DotnetOrderService.Immutables;
using DotnetOrderService.Constants.Cache;
using DotnetOrderService.Constants.Logger;
using DotnetOrderService.Infrastructure.Shareds;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace DotnetOrderService.Infrastructure.Middlewares
{
    public class FlushCacheMiddleware(
        ILoggerFactory logger,
        RequestDelegate next
        )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger _logger = logger.CreateLogger(LoggerConstant.ERROR);

        public async Task Invoke(
            HttpContext context,
            IDistributedCache distributedCache,
            IConfiguration config
        )
        {
            var method = context.Request.Method.ToLower();
            var methodToChecks = new List<string> { "post", "put", "patch", "delete" };
            if (methodToChecks.Exists(methodToCheck => method == methodToCheck))
            {
                _logger.LogInformation("[CACHE] - Cleaing Processing");
                Utils.BackgroundProcessThreadAsync(() => CleanCache(config, distributedCache));
            }

            await _next(context);
        }

        private async Task CleanCache(
            IConfiguration config,
            IDistributedCache distributedCache
        )
        {
            try
            {
                var isRedisEnable = bool.Parse(config["Redis:IsEnable"]);
                var keys = new List<string>();
                if (isRedisEnable)
                {
                    var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
                    {
                        AllowAdmin = true,
                        Password = config["Redis:Password"],
                        EndPoints = { config["Redis:Host"] + ':' + config["Redis:Port"] },
                        Ssl = false
                    });

                    foreach (var endpoint in redis.GetEndPoints())
                    {
                        var server = redis.GetServer(endpoint);
                        keys.AddRange(server.Keys().Where(key => key.ToString().Contains(CacheConstant.REDIS_CACHE_NAME_PREFIX)).Select(key => key.ToString()));
                    }

                    await redis.CloseAsync();
                }
                else
                {
                    keys = Cache.Keys;
                }

                foreach (var key in keys)
                {
                    await distributedCache.RemoveAsync(key);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
            }
        }
    }
}
