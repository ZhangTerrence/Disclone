using Disclone.API.DTOs;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] CredentialsDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(body);
            }

            var accessToken = body.AccessToken;
            var refreshToken = body.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal.Identity?.Name is null)
            {
                return Unauthorized("Unauthorized.");
            }

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (user is null)
            {
                return NotFound("User not found.");
            }
            Console.WriteLine((user.RefreshToken, refreshToken));
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Forbid();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            HttpContext.Response.Cookies.Append("Access", newAccessToken, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true
            });
            HttpContext.Response.Cookies.Append("Refresh", newRefreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true,
                Path = "/api/token/refresh"
            });

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        try
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized("Unauthorized.");
            }
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}