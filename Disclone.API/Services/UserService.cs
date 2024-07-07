using Disclone.API.Data;
using Disclone.API.Interfaces;
using Disclone.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Disclone.API.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(DatabaseContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> FindByName(string userName)
    {
        return await _userManager.Users.FirstOrDefaultAsync(user => user.UserName!.ToLower() == userName.ToLower());
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<bool> CreateUser(ApplicationUser user, string password)
    {
        var createdUser = await _userManager.CreateAsync(user, password);
        foreach (var e in createdUser.Errors.Select(e => e.Description))
        {
            Console.WriteLine(e);
        }

        return createdUser.Succeeded;
    }

    public async Task<bool> AssignUser(ApplicationUser user, string role)
    {
        var assignedRole = await _userManager.AddToRoleAsync(user, role);
        return assignedRole.Succeeded;
    }

    public async Task<bool> ValidateCredentials(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> SaveUpdatedUser(ApplicationUser user)
    {
        var savedUser = await _userManager.UpdateAsync(user);
        return savedUser.Succeeded;
    }
}