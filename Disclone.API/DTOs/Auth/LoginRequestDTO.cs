using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.Auth;

public record LoginRequestDTO
{
    [Required(ErrorMessage = "Username is required.")]
    public required string UserName { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; init; }
}