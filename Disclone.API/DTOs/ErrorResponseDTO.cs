namespace Disclone.API.DTOs;

public record ErrorResponseDTO
{
    public required Dictionary<string, IEnumerable<string>> Errors { get; set; }

    public static ErrorResponseDTO New(List<string?> keys, List<IEnumerable<string>> values)
    {
        var errors = new Dictionary<string, IEnumerable<string>>();

        for (var i = 0; i < keys.Count; i++)
        {
            errors.Add(keys[i] ?? "Unknown", values[i]);
        }

        return new ErrorResponseDTO
        {
            Errors = errors
        };
    }
}