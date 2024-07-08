using System.Security.Claims;
using Disclone.API.DTOs;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userExists = await _userManager.FindByNameAsync(body.UserName);
            if (userExists is not null)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "FindByNameAsync", ["Username has already been taken."] }
                    }
                });
            }

            var user = new ApplicationUser
            {
                UserName = body.UserName,
                Email = body.Email,
                About = "",
                DateCreated = DateTime.Now.ToUniversalTime(),
                DateModified = DateTime.Now.ToUniversalTime()
            };

            var createdUser = await _userManager.CreateAsync(user, body.Password);
            if (!createdUser.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "CreateAsync", createdUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            var assignedUser = await _userManager.AddToRoleAsync(user, "User");
            if (!assignedUser.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "AddToRoleAsync", assignedUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateCookiesFromTokens(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "UpdateAsync", savedUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            return Ok(new CredentialsResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
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

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(body.UserName);
            if (user is null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "FindByNameAsync", ["User not found."] }
                    }
                });
            }

            var validatedCredentials = await _userManager.CheckPasswordAsync(user, body.Password);
            if (!validatedCredentials)
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "CheckPasswordAsync", ["Invalid username or password."] }
                    }
                });
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateCookiesFromTokens(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Errors = new Dictionary<string, IEnumerable<string>>
                    {
                        { "UpdateAsync", savedUser.Errors.Select(e => e.Description) }
                    }
                });
            }

            return Ok(new CredentialsResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
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
}