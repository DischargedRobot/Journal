using MainService;
using Microsoft.EntityFrameworkCore;

namespace MainService.Tests.Helpers;

public static class TestDbContextFactory
{
    public static MainServiceContext Create()
    {
        DbContextOptions<MainServiceContext> options = new DbContextOptionsBuilder<MainServiceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // каждый тест с уникальной бд
            .Options;

        return new MainServiceContext(options);
    }
}