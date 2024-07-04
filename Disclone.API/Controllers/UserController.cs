using Disclone.API.DTOs.User;
using Disclone.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetUsers();
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
        throw new NotImplementedException();
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