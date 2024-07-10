using System.Security.Claims;

namespace Disclone.API.Interfaces;

public interface ITokenService
{
    List<Claim> GenerateClaims(string userName, string role);
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    void GenerateCookiesFromTokens(HttpContext httpContext, string accessToken, string refreshToken);
}