using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.User;

public record StartFriendshipRequestDTO
{
    [Required(ErrorMessage = "Requester id is required.")]
    public required string RequesterId { get; init; }

    [Required(ErrorMessage = "Requestee id is required.")]
    public required string RequesteeId { get; init; }
}