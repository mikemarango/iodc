using Microsoft.EntityFrameworkCore;
using OIDC.IdentityServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.IdentityServer.Data
{
    public class IdsrvDbContext : DbContext
    {
        public IdsrvDbContext(DbContextOptions<IdsrvDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.SubjectId).HasName("Users");
            modelBuilder.Entity<User>().Property(u => u.SubjectId).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Password).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.IsActive).HasMaxLength(100);
            modelBuilder.Entity<User>().HasData(UserData());
            

            modelBuilder.Entity<Login>().HasKey(l => l.Id).HasName("UserLogins");
            modelBuilder.Entity<Login>().Property(l => l.SubjectId).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Login>().Property(l => l.LoginProvider).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<Login>().Property(l => l.ProviderKey).HasMaxLength(250).IsRequired();

            modelBuilder.Entity<Claim>().HasKey(c => c.Id).HasName("Claims");
            modelBuilder.Entity<Claim>().Property(c => c.Id).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Claim>().Property(c => c.SubjectId).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Claim>().Property(c => c.ClaimType).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<Claim>().Property(c => c.ClaimValue).HasMaxLength(250).IsRequired();
            modelBuilder.Entity<Claim>().HasData(ClaimData());

            base.OnModelCreating(modelBuilder);
        }

        private User[] UserData()
        {
            var users = new[]
            {
                new User()
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Frank",
                    Password = "P@ssw0rd!",
                    IsActive = true,
                },
                new User()
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Claire",
                    Password = "P@ssw0rd!",
                    IsActive = true,
                }
            };
            return users;
        }

        private Claim[] ClaimData()
        {
            var claims = new[]
            {
                new Claim("role", "FreeUser") { Id = "d51211d4-48be-11e8-842f-0ed5f89f718b", SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },
                new Claim("given_name", "Frank") { Id = "d512162a-48be-11e8-842f-0ed5f89f718b", SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },
                new Claim("family_name", "Underwood") { Id = "d51218be-48be-11e8-842f-0ed5f89f718b", SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },
                new Claim("address", "1 Main Road") { Id = "d5121e5e-48be-11e8-842f-0ed5f89f718b", SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },
                new Claim("subscriptionlevel", "FreeUser") { Id = "d51220c0-48be-11e8-842f-0ed5f89f718b", SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },
                new Claim("country", "nl") { Id = "d51222e6-48be-11e8-842f-0ed5f89f718b" , SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7" },

                new Claim("role", "PayingUser") { Id = "d5122516-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                new Claim("given_name", "Claire") { Id = "d512273c-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                new Claim("family_name", "Underwood") { Id = "d512294e-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                new Claim("address", "1 Big Street") { Id = "d5122f52-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                new Claim("subscriptionlevel", "PayingUser") { Id = "d5123222-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" },
                new Claim("country", "be") { Id = "d512345c-48be-11e8-842f-0ed5f89f718b", SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7" }
            };

            return claims;
        }
    }
}
