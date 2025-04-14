
using DotNetOrderService.Infrastructure.Exceptions;
using Hanssens.Net;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DotNetOrderService.Infrastructure.Databases
{
    public class LocalStorageDatabase(
        IConfiguration config
        )
    {

        private readonly IConfiguration _config = config;
        private readonly LocalStorage _localStorage = new();

        public async Task Store<T>(string key, T value)
        {
            var isRedisEnable = bool.Parse(_config["Redis:IsEnable"]);
            if (isRedisEnable)
            {
                var redisConfig = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    AllowAdmin = true,
                    Password = _config["Redis:Password"],
                    EndPoints = { _config["Redis:Host"] + ':' + _config["Redis:Port"] },
                    Ssl = false
                });

                var redis = redisConfig.GetDatabase();
                await redis.StringSetAsync(key, JsonConvert.SerializeObject(value));

                await redisConfig.CloseAsync();
            }
            else
            {
                // Store in local storage
                _localStorage.Store(key, JsonConvert.SerializeObject(value));
            }
        }

        public async Task<T> Get<T>(string key)
        {
            var isRedisEnable = bool.Parse(_config["Redis:IsEnable"]);
            if (isRedisEnable)
            {
                var redisConfig = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    AllowAdmin = true,
                    Password = _config["Redis:Password"],
                    EndPoints = { _config["Redis:Host"] + ':' + _config["Redis:Port"] },
                    Ssl = false
                });

                var redis = redisConfig.GetDatabase();
                var strValue = await redis.StringGetAsync(key);

                if (strValue.IsNullOrEmpty)
                {
                    throw new UnauthenticatedException();
                }

                await redisConfig.CloseAsync();
                return JsonConvert.DeserializeObject<T>(strValue);
            }
            else
            {
                // Store in local storage
                try
                {
                    var strValue = _localStorage.Get<string>(key);
                    return JsonConvert.DeserializeObject<T>(strValue);
                }
                catch (Exception)
                {
                    throw new UnauthenticatedException();
                }
            }
        }
    }
}