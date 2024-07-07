using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.User;

public record FriendshipRequestDTO
{
    [Required(ErrorMessage = "Requester id is required.")]
    public required string RequesterId { get; set; }

    [Required(ErrorMessage = "Requestee id is required.")]
    public required string RequesteeId { get; set; }
}