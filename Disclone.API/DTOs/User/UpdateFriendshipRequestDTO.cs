using System.ComponentModel.DataAnnotations;
using Disclone.API.Models;

namespace Disclone.API.DTOs.User;

public record UpdateFriendshipRequestDTO
{
    [Required(ErrorMessage = "UserA id is required.")]
    public required string UserAId { get; init; }

    [Required(ErrorMessage = "UserB id is required.")]
    public required string UserBId { get; init; }

    [Required(ErrorMessage = "Status is required.")]
    public required FriendshipStatus Status { get; init; }
}