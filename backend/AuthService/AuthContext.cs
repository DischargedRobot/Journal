using AuthService.Models;

using Microsoft.EntityFrameworkCore;

namespace AuthService
{
    public class AuthServiceContext : DbContext
    {
        public AuthServiceContext() { }
        public AuthServiceContext(DbContextOptions<AuthServiceContext> options) : base(options) { }

        public DbSet<Users> Users => Set<Users>();
        public DbSet<Roles> Roles => Set<Roles>();
        public DbSet<RoleRights> RoleRights => Set<RoleRights>();
        public DbSet<RefreshTokens> RefreshTokens => Set<RefreshTokens>();
        public DbSet<UserRoles> UserRoles => Set<UserRoles>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoles>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Roles>()
                .HasMany(r => r.RoleRights)
                .WithOne(rr => rr.Role)
                .HasForeignKey(rr => rr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Users>().HasAlternateKey(e => e.Uuid);
            modelBuilder.Entity<Roles>().HasAlternateKey(e => e.Uuid);
            modelBuilder.Entity<RoleRights>().HasAlternateKey(e => e.Uuid);
        }

        private void IncrementVersions()
        {
            foreach (var entry in ChangeTracker.Entries<AuthService.Models.BaseEntity>()
                .Where(e => e.State == EntityState.Added ||
                    (e.State == EntityState.Modified &&
                     e.Properties.Any(p => p.IsModified && p.Metadata.Name != nameof(AuthService.Models.BaseEntity.Version)))))
            {
                entry.Entity.Version++;
            }
        }

        public override int SaveChanges()
        {
            IncrementVersions();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IncrementVersions();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}