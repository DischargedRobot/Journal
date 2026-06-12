using System.Text.Json;

using AuthService.Model;

using StackExchange.Redis;

namespace AuthService.Redis
{

    public class RegistrationCodeData
    {
        public required Roles[] Roles { get; set; }
        public Guid GroupUuid { get; set; }
        public Guid DepartmentUuid { get; set; }
    }

    public class RedisRegistrationCode : IRegistrationCodeStore
    {
        private readonly IDatabase _redis;

        public RedisRegistrationCode(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }
        private static string GetKey(Guid codeUuid) => $"registration-code:{codeUuid}";

        public async Task SaveAsync(Guid codeUuid, RegistrationCodeData data, TimeSpan ttl)
        {
            string dataString = JsonSerializer.Serialize(data);
            string key = GetKey(codeUuid);
            await _redis.StringSetAsync(key, dataString, ttl);
        }

        public async Task<RegistrationCodeData?> GetAsync(Guid codeUuid)
        {
            string key = GetKey(codeUuid);
            RedisValue value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<RegistrationCodeData>(value.ToString());
        }

        public async Task DeleteAsync(Guid codeUuid)
        {
            string key = GetKey(codeUuid);
            await _redis.KeyDeleteAsync(key);
        }

    }
}