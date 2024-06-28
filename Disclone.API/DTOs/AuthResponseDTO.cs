namespace Disclone.API.DTOs;

public class AuthResponseDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}