using Microsoft.EntityFrameworkCore;

namespace MainService
{
    public class MainServiceContext : DbContext
    {
        public MainServiceContext() { }

        public MainServiceContext(DbContextOptions<MainServiceContext> options) : base(options) { }

    }
}