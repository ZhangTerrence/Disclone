using Disclone.API.DTOs.User;
using Disclone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [Route("friend/request")]
    public async Task<IActionResult> SendFriendRequest([FromBody] FriendshipDTO body)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(body);
            }

            var requester = await _userManager.FindByIdAsync(body.RequesterId);
            if (requester is null)
            {
                return NotFound("Requester not found");
            }
            
            var requestee = await _userManager.FindByIdAsync(body.RequesteeId);
            if (requestee is null)
            {
                return NotFound("Requestee not found");
            }

            var friendship = new NewFriendshipDTO
            {
                UserAId = requester.Id,
                UserBId = requestee.Id,
                Status = FriendshipStatus.PENDING
            };
            
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPatch]
    [Route("friend/accept")]
    public async Task<IActionResult> AcceptFriendRequest()
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("friend/reject")]
    public async Task<IActionResult> RejectFriendRequest()
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("friend/remove")]
    public async Task<IActionResult> RemoveFriend()
    {
        throw new NotImplementedException();
    }
}