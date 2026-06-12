using AuthService.Lib.Utils;
using AuthService.Redis;

namespace AuthService.Tests.Helpers;

public static class AuthTestHelper
{
    
    public const string JwtSecretKey = "test-jwt-secret-key-for-unit-tests!";

    public static TokenService CreateTokenService() => new(JwtSecretKey);

    public static async Task<string> IssueOpaqueTokenAsync(
        TokenService tokenService,
        ITokenStore store,
        Guid userUuid,
        IEnumerable<string>? rights = null)
    {
        Guid tokenUuid = Guid.NewGuid();
        string accessToken = tokenService.GenerateAccessToken(tokenUuid, userUuid, rights ?? []);
        string opaqueToken = tokenService.GenerateOpaqueToken(tokenUuid);
        await store.SaveAsync(tokenUuid, accessToken, TimeSpan.FromMinutes(30));
        return opaqueToken;
    }
}
