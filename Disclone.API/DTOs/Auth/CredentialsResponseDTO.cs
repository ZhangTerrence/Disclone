namespace Disclone.API.DTOs.Auth;

public record CredentialsResponseDTO
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}