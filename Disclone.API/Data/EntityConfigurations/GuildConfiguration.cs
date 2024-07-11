using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disclone.API.Data.EntityConfigurations;

public class GuildConfiguration : IEntityTypeConfiguration<Guild>
{
    public void Configure(EntityTypeBuilder<Guild> builder)
    {
        builder.ToTable("Guilds");
        
        // Configures primary key.
        builder.HasKey(e => e.GuildId);

        // Configures properties.
        builder.Property(e => e.Name).IsRequired().HasMaxLength(31);
        builder.Property(e => e.Description).IsRequired().HasDefaultValue("").HasMaxLength(255);
        
        // Configures relations.
        builder.HasOne(e => e.Owner).WithMany(e => e.OwnedGuilds).HasForeignKey(e => e.OwnerId);
    }
}