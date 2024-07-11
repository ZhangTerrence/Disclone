using Disclone.API.DTOs;
using Disclone.API.DTOs.User;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Disclone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public UserController(UserManager<User> userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userRepository.GetUsers();

            return Ok(users.Select(user => user.ToUserResponseDTO()));
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }

    [HttpGet]
    [Authorize]
    [Route("{userName}")]
    public async Task<IActionResult> GetUserByName([FromRoute] string userName)
    {
        try
        {
            var user = await _userRepository.GetUserByName(userName);
            if (user is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserRepository.GetUserByName"], [["User not found."]]));
            }
            
            Console.WriteLine(user.Friendships.FirstOrDefault()?.UserB?.UserName ?? "Null");

            return Ok(user.ToUserResponseDTO());
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }

    [HttpPost]
    [Authorize]
    [Route("friend")]
    public async Task<IActionResult> StartFriendship([FromBody] StartFriendshipRequestDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var requester = await _userManager.FindByIdAsync(body.RequesterId);
            if (requester is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var requestee = await _userManager.FindByIdAsync(body.RequesteeId);
            if (requestee is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var friendship = await _userRepository.GetFriendship(requester, requestee);
            if (friendship is not null)
            {
                return BadRequest(ErrorResponseDTO.New(["UserRepository.GetFriendship"],
                    [["Friend request has already been sent."]]));
            }

            await _userRepository.CreateFriendship(requester, requestee);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }

    [HttpPatch]
    [Route("friend")]
    public async Task<IActionResult> UpdateFriendship([FromBody] UpdateFriendshipRequestDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userA = await _userManager.FindByIdAsync(body.UserAId);
            if (userA is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var userB = await _userManager.FindByIdAsync(body.UserBId);
            if (userB is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var friendship = await _userRepository.GetFriendship(userA, userB);
            if (friendship is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserRepository.GetFriendship"], [["No friend request found."]]));
            }

            await _userRepository.UpdateFriendship(userA, userB, body.Status);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }

    [HttpDelete]
    [Route("friend")]
    public async Task<IActionResult> EndFriendship([FromBody] EndFriendshipRequestDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userA = await _userManager.FindByIdAsync(body.UserAId);
            if (userA is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var userB = await _userManager.FindByIdAsync(body.UserBId);
            if (userB is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserManager.FindByIdAsync"], [["User not found."]]));
            }

            var friendship = await _userRepository.GetFriendship(userA, userB);
            if (friendship is null)
            {
                return NotFound(ErrorResponseDTO.New(["UserRepository.GetFriendship"], [["No friend request found."]]));
            }

            await _userRepository.DeleteFriendship(userA, userB);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, ErrorResponseDTO.New([e.Source], [[e.Message]]));
        }
    }
}