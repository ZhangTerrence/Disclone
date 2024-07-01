namespace Disclone.API.DTOs.Auth;

public class CredentialsDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}