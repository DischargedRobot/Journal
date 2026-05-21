
using System.Text.Json;

using StackExchange.Redis;

namespace AuthService.Redis
{
    public class RedisAccessTokenkList : ITokenStore
    {
        private readonly IDatabase _redis;

        public RedisAccessTokenkList(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        private static string GetKey(Guid tokenUuid) => $"opaqueaccess:{tokenUuid}";

        public async Task SaveAsync(Guid tokenUuid, string token, TimeSpan ttl)
        {
            string key = GetKey(tokenUuid);
            await _redis.StringSetAsync(key, token, ttl);
        }

        public async Task<string?> GetAsync(Guid tokenUuid)
        {
            string key = GetKey(tokenUuid);
            RedisValue value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return value.ToString();
        }

        public async Task DeleteAsync(Guid tokenUuid)
        {
            string key = GetKey(tokenUuid);
            await _redis.KeyDeleteAsync(key);
        }
    }
}