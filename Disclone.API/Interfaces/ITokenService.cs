using System.Security.Claims;

namespace Disclone.API.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    void GenerateBothCookies(HttpContext httpContext, string accessToken, string refreshToken);
}