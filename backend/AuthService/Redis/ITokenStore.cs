namespace AuthService.Redis
{
    public interface ITokenBlackListStore
    {
        Task SaveAsync(Guid tokenUUID, Guid userUuid, TimeSpan ttl);
        Task<string?> GetAsync(Guid tokenUUID);
        Task DeleteAsync(Guid tokenUUID);
    }

    public interface IAccessTokenBlackListStore : ITokenBlackListStore;

    public interface IRefreshTokenBlackListStore : ITokenBlackListStore;

    public interface ITokenStore
    {
        Task SaveAsync(Guid tokenUUID, string data, TimeSpan ttl);
        Task<string?> GetAsync(Guid tokenUUID);
        Task DeleteAsync(Guid tokenUUID);
    }
}