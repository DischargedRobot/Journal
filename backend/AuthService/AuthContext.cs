using AuthService.Model;

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
        public DbSet<Sessions> Sessions => Set<Sessions>();
        public DbSet<RolesTypes> RolesTypes => Set<RolesTypes>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                j => j.HasOne<Roles>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<Users>().WithMany().HasForeignKey("UserId")
            );


            modelBuilder.Entity<Roles>()
                .HasMany(r => r.RoleRights)
                .WithMany(rr => rr.Roles)
                .UsingEntity<Dictionary<string, object>>(
                "RoleRoleRights",
                j => j.HasOne<RoleRights>().WithMany().HasForeignKey("RoleRightId"),
                j => j.HasOne<Roles>().WithMany().HasForeignKey("RoleId")
            );


            modelBuilder.Entity<Users>().HasAlternateKey(e => e.Uuid);
            modelBuilder.Entity<Roles>().HasAlternateKey(e => e.Uuid);
            modelBuilder.Entity<Roles>() // только одна базовая роль
                .HasIndex(r => r.IsBase)
                .IsUnique()
                .HasFilter("\"IsBase\" = true");
            modelBuilder.Entity<RoleRights>().HasAlternateKey(e => e.Uuid);
        }

        private void IncrementVersions()
        {
            foreach (var entry in ChangeTracker.Entries<AuthService.Model.BaseEntity>()
                .Where(e => e.State == EntityState.Added ||
                    (e.State == EntityState.Modified &&
                     e.Properties.Any(p => p.IsModified && p.Metadata.Name != nameof(AuthService.Model.BaseEntity.Version)))))
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