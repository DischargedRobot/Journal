using AuthService.Redis;
using AuthService.Model;

namespace AuthService.Tests.Helpers;

public sealed class InMemoryTokenStore : ITokenStore
{
    // имитация редиса
    private readonly Dictionary<Guid, string> _store = [];

    public Task SaveAsync(Guid tokenUuid, string data, TimeSpan ttl)
    {
        _store[tokenUuid] = data;
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(Guid tokenUuid) =>
        Task.FromResult(_store.TryGetValue(tokenUuid, out string? value) ? value : null);

    public Task DeleteAsync(Guid tokenUuid)
    {
        _store.Remove(tokenUuid);
        return Task.CompletedTask;
    }
}

public sealed class InMemoryTokenBlackListStore : IAccessTokenBlackListStore, IRefreshTokenBlackListStore
{
    private readonly Dictionary<Guid, string> _store = [];

    public Task SaveAsync(Guid tokenUuid, Guid userUuid, TimeSpan ttl)
    {
        _store[tokenUuid] = userUuid.ToString();
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(Guid tokenUuid) =>
        Task.FromResult(_store.TryGetValue(tokenUuid, out string? value) ? value : null);

    public Task DeleteAsync(Guid tokenUuid)
    {
        _store.Remove(tokenUuid);
        return Task.CompletedTask;
    }
}

public sealed class InMemoryRegistrationCodeStore : IRegistrationCodeStore
{
    private readonly Dictionary<Guid, RegistrationCodeData> _store = [];

    public Task SaveAsync(Guid codeUuid, RegistrationCodeData data, TimeSpan ttl)
    {
        _store[codeUuid] = data;
        return Task.CompletedTask;
    }

    public Task<RegistrationCodeData?> GetAsync(Guid codeUuid) =>
        Task.FromResult(_store.TryGetValue(codeUuid, out RegistrationCodeData? value) ? value : null);

    public Task DeleteAsync(Guid codeUuid)
    {
        _store.Remove(codeUuid);
        return Task.CompletedTask;
    }
}
