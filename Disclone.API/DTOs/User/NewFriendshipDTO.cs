using Disclone.API.Models;

namespace Disclone.API.DTOs.User;

public class NewFriendshipDTO
{
    public required Guid UserAId { get; set; }
    public required Guid UserBId { get; set; }
    public required FriendshipStatus Status { get; set; }
}