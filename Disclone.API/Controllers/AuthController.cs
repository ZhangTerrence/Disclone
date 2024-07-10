using Disclone.API.DTOs;
using Disclone.API.DTOs.Auth;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Disclone.API.Services;
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
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager, ITokenService tokenService)
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
                return BadRequest(ErrorResponseDTO.New(["FindByNameAsync"], [["Username has already been taken."]]));
            }

            var user = body.ToUser();

            var createdUser = await _userManager.CreateAsync(user, body.Password);
            if (!createdUser.Succeeded)
            {
                return BadRequest(ErrorResponseDTO.New(["CreateAsync"],
                    [createdUser.Errors.Select(e => e.Description)]));
            }

            var assignedUser = await _userManager.AddToRoleAsync(user, "User");
            if (!assignedUser.Succeeded)
            {
                return BadRequest(ErrorResponseDTO.New(["AddToRoleAsync"],
                    [assignedUser.Errors.Select(e => e.Description)]));
            }

            var accessToken = _tokenService.GenerateAccessToken(_tokenService.GenerateClaims(body.UserName, "User"));
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateCookiesFromTokens(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return StatusCode(500,
                    ErrorResponseDTO.New(["UpdateAsync"], [savedUser.Errors.Select(e => e.Description)]));
            }

            return Ok(CredentialsResponseDTO.New(accessToken, refreshToken));
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
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
                return NotFound(ErrorResponseDTO.New(["FindByNameAsync"], [["User not found."]]));
            }

            var validatedCredentials = await _userManager.CheckPasswordAsync(user, body.Password);
            if (!validatedCredentials)
            {
                return Unauthorized(ErrorResponseDTO.New(["CheckPasswordAsync"], [["Invalid username or password."]]));
            }

            var accessToken = _tokenService.GenerateAccessToken(_tokenService.GenerateClaims(body.UserName, "User"));
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.GenerateCookiesFromTokens(HttpContext, accessToken, refreshToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();

            var savedUser = await _userManager.UpdateAsync(user);
            if (!savedUser.Succeeded)
            {
                return StatusCode(500,
                    ErrorResponseDTO.New(["UpdateAsync"], [savedUser.Errors.Select(e => e.Description)]));
            }

            return Ok(CredentialsResponseDTO.New(accessToken, refreshToken));
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }
}