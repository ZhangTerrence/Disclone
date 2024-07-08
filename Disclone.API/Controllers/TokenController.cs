using Disclone.API.DTOs;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/token")]
public class TokenController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public TokenController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Authorize]
    [Route("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }
            
            
            if (HttpContext.Items["Refresh"] is not string refreshToken)
            {
                return Unauthorized();
            }
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "FindByName", ["User not found."] }
                    }
                });
            }

            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Forbid();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(User.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateCookiesFromTokens(HttpContext, newAccessToken, newRefreshToken);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return StatusCode(500, new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "SaveUpdatedUser", savedUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            return Ok(new CredentialsResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new ErrorResponseDTO
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    { e.Source ?? "UnknownSource", [e.Message] }
                }
            });
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
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "FindByName", ["User not found."] }
                    }
                });
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return StatusCode(500, new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "SaveUpdatedUser", savedUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, new ErrorResponseDTO
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    { e.Source ?? "UnknownSource", [e.Message] }
                }
            });
        }
    }
}