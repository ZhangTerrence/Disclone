using System.ComponentModel.DataAnnotations;

namespace Disclone.API.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "UserName is required.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}