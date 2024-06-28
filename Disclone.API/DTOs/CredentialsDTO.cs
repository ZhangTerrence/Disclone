namespace Disclone.API.DTOs;

public class CredentialsDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}