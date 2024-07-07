namespace Disclone.API.DTOs;

public record ErrorResponseDTO
{
    public required Dictionary<string, IEnumerable<string>> Errors { get; set; }
}