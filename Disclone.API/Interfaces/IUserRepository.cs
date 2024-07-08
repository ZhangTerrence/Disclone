using Disclone.API.Models;

namespace Disclone.API.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsers();
}