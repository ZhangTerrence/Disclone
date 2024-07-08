using Microsoft.AspNetCore.Identity;

namespace Disclone.API;

public class IdentityErrors : IdentityErrorDescriber
{
    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = $"Username '{userName}' is invalid, can only contain letters, digits, or underscores."
        };
    }
}