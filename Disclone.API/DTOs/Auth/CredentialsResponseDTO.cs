namespace Disclone.API.DTOs.Auth;

public record CredentialsResponseDTO
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }

    public static CredentialsResponseDTO New(string accessToken, string refreshToken)
    {
        return new CredentialsResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}