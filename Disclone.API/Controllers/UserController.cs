using Disclone.API.DTOs;
using Disclone.API.DTOs.User;
using Disclone.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Disclone.API.Controllers;

[ApiController]
[Route("/api/user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
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
    [Route("friend/request")]
    public async Task<IActionResult> SendFriendRequest([FromBody] FriendshipRequestDTO body)
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