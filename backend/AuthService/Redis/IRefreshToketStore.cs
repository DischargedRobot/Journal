public interface IRefreshTokenStore
{
    Task SaveAsync(Guid tokenUUID, Guid userUuid, string tokenHash, TimeSpan ttl);
    Task<string?> GetHashAsync(Guid tokenUUID);
    Task DeleteAsync(Guid tokenUUID);
}