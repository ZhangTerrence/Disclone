using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "UserName is required.")]
    public required string UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}