namespace Disclone.API.DTOs.User;

public record UserResponseDTO
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string About { get; init; }
    public required DateTime DateCreated { get; init; }
    public required DateTime DateModified { get; init; }
    public required List<string> Friends { get; init; }
}