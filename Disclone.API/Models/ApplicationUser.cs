using Microsoft.AspNetCore.Identity;

namespace Disclone.API.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string About { get; set; }
    public required DateTime DateCreated { get; init; }
    public required DateTime DateModified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<Friendship> Requesters { get; set; } = [];
    public ICollection<Friendship> Requestees { get; set; } = [];
}