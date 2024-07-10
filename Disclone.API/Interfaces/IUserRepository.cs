using Disclone.API.Models;

namespace Disclone.API.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User?> GetUserByName(string userName);
    Task<IEnumerable<Friendship>?> GetFriendship(User userA, User userB);
    Task CreateFriendship(User userA, User userB);
    Task UpdateFriendship(User userA, User userB, FriendshipStatus status);
    Task DeleteFriendship(User userA, User userB);
}