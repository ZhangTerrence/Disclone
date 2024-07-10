using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.Auth;

public record RegisterRequestDTO
{
    [Required(ErrorMessage = "Username is required.")]
    public required string UserName { get; init; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; init; }
}