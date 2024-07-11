using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disclone.API.Data.EntityConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(e => e.SubscriptionId);
        
        // Configures properties.
        builder.Property(e => e.Status).IsRequired().HasDefaultValue(SubscriptionStatus.Member);
        builder.Property(e => e.Permissions).HasMaxLength(31).HasDefaultValue(null);
        
        // Configures relations.
        builder.HasOne(e => e.User).WithMany(e => e.Guilds).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Guild).WithMany(e => e.Users).HasForeignKey(e => e.GuildId);
    }
}