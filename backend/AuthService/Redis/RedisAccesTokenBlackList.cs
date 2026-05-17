
using System.Text.Json;

using StackExchange.Redis;

namespace AuthService.Redis
{
    public class RedisAccessTokenBlackList : ITokenStore
    {
        private readonly IDatabase _redis;

        public RedisAccessTokenBlackList(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        private static string GetKey(Guid tokenUuid) => $"access:blacklist:{tokenUuid}";

        public async Task SaveAsync(Guid tokenUuid, Guid userUuid, TimeSpan ttl)
        {
            string value = JsonSerializer.Serialize(new { UserUuid = userUuid, RevokedAt = DateTime.UtcNow });
            string key = GetKey(tokenUuid);
            await _redis.StringSetAsync(key, value, ttl);
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