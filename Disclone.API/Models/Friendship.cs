namespace Disclone.API.Models;

public enum FriendshipStatus
{
    PENDING,
    FRIENDS,
    BLOCKED
}

public class Friendship
{
    public required Guid UserAId { get; set; }
    public required ApplicationUser UserA { get; set; }
    public required Guid UserBId { get; set; }
    public required ApplicationUser UserB { get; set; }
    public required FriendshipStatus Status { get; set; }
}