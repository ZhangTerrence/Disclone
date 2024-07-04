using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Disclone.API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Disclone.API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _securityKey;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"],
            Expires = DateTime.Now.AddHours(1).ToUniversalTime(),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = _securityKey
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }

    public void GenerateBothCookies(HttpContext httpContext, string accessToken, string refreshToken)
    {
        httpContext.Response.Cookies.Append("Access", accessToken, new CookieOptions
        {
            Expires = DateTime.Now.AddHours(1),
            Secure = true,
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true
        });
        httpContext.Response.Cookies.Append("Refresh", refreshToken, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1),
            Secure = true,
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            Path = "/api/token/refresh"
        });
    }
}