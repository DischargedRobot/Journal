using AuthService;
using AuthService.Lib.Utils;
using AuthService.Model;

namespace AuthService.Tests.Helpers;

public static class TestDataMock
{
    public static async Task<RolesTypes> MockRoleTypeAsync(AuthServiceContext context)
    {
        RolesTypes roleType = new()
        {
            Name = "Студент"
        };
        context.RolesTypes.Add(roleType);
        await context.SaveChangesAsync();
        return roleType;
    }

    public static async Task<Roles> MockRoleAsync(AuthServiceContext context)
    {
        RolesTypes roleType = await MockRoleTypeAsync(context);

        Roles role = new()
        {
            Name = "Студент",
            RoleType = [roleType],
            RoleRights = []
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    public static async Task<Users> MockUserAsync(AuthServiceContext context)
    {
        Users user = new()
        {
            Login = "ivanov",
            PasswordHash = HashingPassword.ComputeHash("password123"),
            FirstName = "Иван",
            LastName = "Иванов",
            Patronymic = "Иванович",
            Email = "ivanov@example.com",
            Roles = []
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static async Task<Sessions> MockSessionAsync(AuthServiceContext context)
    {
        Users user = await MockUserAsync(context);

        Sessions session = new()
        {
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RefreshTokenUuid = Guid.NewGuid(),
            UserId = user.UserId,
            User = user,
            UserAgent = "TestAgent",
            BrowserName = "Chrome",
            BrowserVersion = "120",
            OsName = "Windows"
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
        return session;
    }
}
