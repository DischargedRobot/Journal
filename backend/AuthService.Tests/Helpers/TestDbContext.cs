using AuthService;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Helpers;

public sealed class AuthTestContext : IDisposable
{
    public AuthServiceContext Context { get; }

    private readonly SqliteConnection _connection;

    public AuthTestContext()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        DbContextOptions<AuthServiceContext> options = new DbContextOptionsBuilder<AuthServiceContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AuthServiceContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

public static class TestDbContextFactory
{
    public static AuthTestContext Create() => new();
}
