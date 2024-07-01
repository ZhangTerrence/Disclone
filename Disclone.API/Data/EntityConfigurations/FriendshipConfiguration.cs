using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disclone.API.Data.EntityConfigurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        // Configures table name.
        builder.ToTable("Friendship");

        // Configures primary composite key.
        builder.HasKey(e => new { e.UserAId, e.UserBId });

        // Configures properties.
        builder.Property(e => e.Status).HasConversion<string>();

        // Configures relations.
        builder.HasOne(e => e.UserA).WithMany(e => e.Requesters).HasForeignKey(e => e.UserAId);
        builder.HasOne(e => e.UserB).WithMany(e => e.Requestees).HasForeignKey(e => e.UserBId);
    }
}