using Disclone.API.Data;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Services;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
    }
    

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }
}