using Disclone.API.Data;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users
            .Include(e => e.Friendships)
            .ToListAsync();
    }

    public async Task<User?> GetUserByName(string userName)
    {
        var x = await _context.Users
            .Include(e => e.Friendships)
            .ToListAsync();
        return x.FirstOrDefault(e => string.Equals(e.UserName!, userName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task<IEnumerable<Friendship>?> GetFriendship(User userA, User userB)
    {
        var friendship = await _context.Friendships
            .Where(e => (e.UserAId == userA.Id && e.UserBId == userB.Id) ||
                        (e.UserAId == userB.Id && e.UserBId == userA.Id))
            .ToListAsync();
        
        return friendship.Count != 2 ? null : friendship.Take(2);
    }

    public async Task CreateFriendship(User userA, User userB)
    {
        userA.Friendships.Add(new Friendship
        {
            Status = FriendshipStatus.Pending,
            UserA = userA,
            UserB = userB
        });
        userB.Friendships.Add(new Friendship
        {
            Status = FriendshipStatus.Pending,
            UserA = userB,
            UserB = userA
        });
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFriendship(User userA, User userB, FriendshipStatus status)
    {
        var friendship = await GetFriendship(userA, userB);
        if (friendship is null)
        {
            return;
        }

        foreach (var entity in friendship)
        {
            entity.Status = status;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteFriendship(User userA, User userB)
    {
        var friendship = await GetFriendship(userA, userB);
        if (friendship is null)
        {
            return;
        }
        
        foreach (var entity in friendship)
        {
            _context.Friendships.Remove(entity);
        }
        
        await _context.SaveChangesAsync();
    }
}