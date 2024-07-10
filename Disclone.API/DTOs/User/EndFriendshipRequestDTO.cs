using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.User;

public class EndFriendshipRequestDTO
{
    [Required(ErrorMessage = "UserA id is required.")]
    public required string UserAId { get; init; }

    [Required(ErrorMessage = "UserB id is required.")]
    public required string UserBId { get; init; }
}