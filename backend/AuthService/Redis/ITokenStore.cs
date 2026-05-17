namespace AuthService.Redis
{
    public interface ITokenStore
    {
        Task SaveAsync(Guid tokenUUID, Guid userUuid, TimeSpan ttl);
        Task<string?> GetAsync(Guid tokenUUID);
        Task DeleteAsync(Guid tokenUUID);
    }
}