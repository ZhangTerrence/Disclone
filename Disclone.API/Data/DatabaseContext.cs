using Disclone.API.Data.EntityConfigurations;
using Disclone.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Data;

public class DatabaseContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e => e.ToTable("Users"));
        builder.Entity<IdentityRole<Guid>>(e => e.ToTable("Roles"));
        builder.Entity<IdentityUserRole<Guid>>(e => e.ToTable("UserRoles"));
        builder.Entity<IdentityUserClaim<Guid>>(e => e.ToTable("UserClaims"));
        builder.Entity<IdentityUserLogin<Guid>>(e => e.ToTable("UserLogins"));
        builder.Entity<IdentityUserToken<Guid>>(e => e.ToTable("UserTokens"));
        builder.Entity<IdentityRoleClaim<Guid>>(e => e.ToTable("RoleClaims"));
        
        builder.Entity<IdentityRole<Guid>>().HasData([
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                NormalizedName = "ADMIN"
            }
        ]);
        
        new ApplicationUserConfiguration().Configure(builder.Entity<ApplicationUser>());
    }
}