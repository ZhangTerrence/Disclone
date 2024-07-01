using System.Security.Claims;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
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

            var userExists = await _userManager.FindByNameAsync(body.UserName);
            if (userExists is not null)
            {
                return BadRequest("UserName already taken.");
            }

            var user = new ApplicationUser
            {
                UserName = body.UserName,
                Email = body.Email,
                About = "",
                DateCreated = DateTime.Now.ToUniversalTime(),
                DateModified = DateTime.Now.ToUniversalTime()
            };

            var registeredUser = await _userManager.CreateAsync(user, body.Password);
            if (!registeredUser.Succeeded)
            {
                return StatusCode(500, registeredUser.Errors);
            }

            var registeredRole = await _userManager.AddToRoleAsync(user, "User");
            if (!registeredRole.Succeeded)
            {
                return StatusCode(500, registeredRole.Errors);
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();

            HttpContext.Response.Cookies.Append("Access", accessToken, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true
            });
            HttpContext.Response.Cookies.Append("Refresh", refreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true,
                Path = "/api/token/refresh"
            });

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            await _userManager.UpdateAsync(user);

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

            var user = await _userManager.Users.FirstOrDefaultAsync(user =>
                user.UserName!.ToLower() == body.UserName.ToLower());
            if (user is null)
            {
                return NotFound("User not found");
            }

            var validCredentials = await _signInManager.CheckPasswordSignInAsync(user, body.Password, false);
            if (!validCredentials.Succeeded)
            {
                return Unauthorized("Invalid username or password.");
            }

            var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new(ClaimTypes.Name, body.UserName),
                new(ClaimTypes.Role, "User")
            });
            var refreshToken = _tokenService.GenerateRefreshToken();

            HttpContext.Response.Cookies.Append("Access", accessToken, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true
            });
            HttpContext.Response.Cookies.Append("Refresh", refreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                IsEssential = true,
                Path = "/api/token/refresh"
            });

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            await _userManager.UpdateAsync(user);

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