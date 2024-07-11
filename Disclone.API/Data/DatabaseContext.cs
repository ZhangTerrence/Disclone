using Disclone.API.Data.EntityConfigurations;
using Disclone.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Data;

public class DatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(e => e.ToTable("Users"));
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

        new UserConfiguration().Configure(builder.Entity<User>());
        new FriendshipConfiguration().Configure(builder.Entity<Friendship>());
        new GuildConfiguration().Configure(builder.Entity<Guild>());
        new SubscriptionConfiguration().Configure(builder.Entity<Subscription>());
    }
}