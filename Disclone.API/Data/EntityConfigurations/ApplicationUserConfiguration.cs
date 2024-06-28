using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Disclone.API.Data.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Configures properties
        builder.Property(e => e.Id).HasColumnName("UserId");
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(31);
        builder.Property(e => e.About).IsRequired().HasDefaultValue("").HasMaxLength(255);
        builder.Property(e => e.DateCreated).IsRequired().HasDefaultValue(DateTime.Now.ToUniversalTime());
        builder.Property(e => e.DateModified).IsRequired().HasDefaultValue(DateTime.Now.ToUniversalTime());
        builder.Property(e => e.RefreshToken).HasDefaultValue(null);
        builder.Property(e => e.RefreshTokenExpiryTime).HasDefaultValue(null);
    }
}