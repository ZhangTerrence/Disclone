using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "Username is required.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}