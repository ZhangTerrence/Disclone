using Disclone.API.DTOs.Auth;
using Disclone.API.DTOs.User;
using Disclone.API.Models;

namespace Disclone.API.Services;

public static class MapperService
{
    public static User ToUser(this RegisterRequestDTO registerRequestDTO)
    {
        return new User
        {
            UserName = registerRequestDTO.UserName,
            Email = registerRequestDTO.Email,
            About = "",
            DateCreated = DateTime.Now.ToUniversalTime(),
            DateModified = DateTime.Now.ToUniversalTime()
        };
    }

    public static UserResponseDTO ToUserResponseDTO(this User user)
    {
        return new UserResponseDTO
        {
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            About = user.About,
            DateCreated = user.DateCreated,
            DateModified = user.DateModified,
            Friends = user.Friendships
                .Where(e => e.Status == FriendshipStatus.Friends)
                .Select(e => e.UserB!.UserName!)
                .ToList()
        };
    }
}