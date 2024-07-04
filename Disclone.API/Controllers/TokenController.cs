using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public TokenController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
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
            if (principal?.Identity?.Name is null)
            {
                return Unauthorized("Unauthorized.");
            }

            var user = await _userService.FindByName(principal.Identity.Name);
            if (user is null)
            {
                return NotFound("User not found.");
            }

            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Forbid();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateBothCookies(HttpContext, newAccessToken, newRefreshToken);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userService.SaveUpdatedUser(user);
            if (!savedUser)
            {
                return StatusCode(500, "Unable to save user.");
            }

            return Ok(new CredentialsDTO
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

            var user = await _userService.FindByName(User.Identity.Name);
            if (user is null)
            {
                return NotFound("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var savedUser = await _userService.SaveUpdatedUser(user);
            if (!savedUser)
            {
                return StatusCode(500, "Unable to save user.");
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}