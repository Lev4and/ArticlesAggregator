namespace ObjectStorage.Abstracts.Models;

public record GetObjectResponse
{
    public string Key { get; init; } = null!;
}