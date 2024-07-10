using Microsoft.AspNetCore.Identity;

namespace Disclone.API.Models;

public class User : IdentityUser<Guid>
{
    public required string About { get; set; }
    public required DateTime DateCreated { get; set; }
    public required DateTime DateModified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<Friendship> Friendships { get; set; } = [];
    public ICollection<Friendship> FriendshipsOf { get; set; } = [];
}