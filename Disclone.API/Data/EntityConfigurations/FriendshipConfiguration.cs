using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disclone.API.Data.EntityConfigurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("Friendships");

        // Configures primary key.
        builder.HasKey(e => e.FriendshipId);
        
        // Configures properties.
        builder.Property(e => e.Status).IsRequired().HasDefaultValue(FriendshipStatus.Pending);

        // Configures relations.
        builder
            .HasOne(e => e.UserA)
            .WithMany(e => e.Friendships)
            .HasForeignKey(e => e.UserAId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(e => e.UserB)
            .WithMany(e => e.FriendshipsOf)
            .HasForeignKey(e => e.UserBId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}