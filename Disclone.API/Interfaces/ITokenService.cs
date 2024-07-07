using System.Security.Claims;

namespace Disclone.API.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    void GenerateCookiesFromTokens(HttpContext httpContext, string accessToken, string refreshToken);
}