namespace Disclone.API.Models;

public enum FriendshipStatus
{
    Pending,
    Friends,
    Blocked
}

public class Friendship
{
    public Guid FriendshipId { get; set; }
    public Guid? UserAId { get; set; }
    public Guid? UserBId { get; set; }
    public FriendshipStatus Status { get; set; }

    public User? UserA { get; set; }
    public User? UserB { get; set; }
}