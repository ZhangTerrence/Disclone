using System.Security.Claims;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;


    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(body);
            }

            var userExists = await _userService.FindByName(body.UserName);
            if (userExists is not null)
            {
                return BadRequest("Username has already been taken.");
            }

            var user = new ApplicationUser
            {
                UserName = body.UserName,
                Email = body.Email,
                About = "",
                DateCreated = DateTime.Now.ToUniversalTime(),
                DateModified = DateTime.Now.ToUniversalTime()
            };

            var createdUser = await _userService.CreateUser(user, body.Password);
            if (!createdUser)
            {
                return StatusCode(500, "Unable to create a new user.");
            }

            var assignedUser = await _userService.AssignUser(user, "User");
            if (!assignedUser)
            {
                return StatusCode(500, "Unable to assign user to USER role.");
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateBothCookies(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userService.SaveUpdatedUser(user);
            if (!savedUser)
            {
                return StatusCode(500, "Unable to save user.");
            }

            return Ok(new CredentialsDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(body);
            }

            var user = await _userService.FindByName(body.UserName);
            if (user is null)
            {
                return NotFound("User not found");
            }

            var validatedCredentials = await _userService.ValidateCredentials(user, body.Password);
            if (!validatedCredentials)
            {
                return Unauthorized("Invalid username or password.");
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateBothCookies(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userService.SaveUpdatedUser(user);
            if (!savedUser)
            {
                return StatusCode(500, "Unable to save user.");
            }

            return Ok(new CredentialsDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}