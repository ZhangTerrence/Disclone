using Disclone.API.Models;

namespace Disclone.API.DTOs.User;

public record FriendshipResponseDTO
{
    public required Guid UserAId { get; set; }
    public required Guid UserBId { get; set; }
    public required FriendshipStatus Status { get; set; }
}