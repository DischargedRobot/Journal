using System.Text.Json;

using StackExchange.Redis;

public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _redis;

    public RedisRefreshTokenStore(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    private static string GetKey(Guid tokenUuid) => $"refresh:{tokenUuid}";

    public async Task SaveAsync(Guid tokenUuid, Guid userUuid, string tokenHash, TimeSpan ttl)
    {
        string value = JsonSerializer.Serialize(new { UserUuid = userUuid, TokenHash = tokenHash });
        await _redis.StringSetAsync(GetKey(tokenUuid), value, ttl);
    }

    public async Task<string?> GetHashAsync(Guid tokenUuid)
    {
        string key = GetKey(tokenUuid);
        RedisValue value = await _redis.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        JsonElement obj = JsonSerializer.Deserialize<JsonElement>(value!);
        return obj.TryGetProperty("TokenHash", out JsonElement tokenHash) ? tokenHash.GetString() : null;
    }

    public async Task DeleteAsync(Guid tokenUuid)
    {
        string key = GetKey(tokenUuid);
        await _redis.KeyDeleteAsync(key);
    }
}