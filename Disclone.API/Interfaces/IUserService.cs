using Disclone.API.Models;

namespace Disclone.API.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> FindByName(string userName);
    Task<IEnumerable<ApplicationUser>> GetUsers();
    Task<bool> CreateUser(ApplicationUser user, string password);
    Task<bool> AssignUser(ApplicationUser user, string role);
    Task<bool> ValidateCredentials(ApplicationUser user, string password);
    Task<bool> SaveUpdatedUser(ApplicationUser user);
}