namespace ObjectStorage.Abstracts.Models;

public record CreateObjectRequest
{
    public string BucketName { get; init; } = null!;
    
    public string Key { get; init; } = null!;

    public string ContentType { get; init; } = null!;

    public Stream Stream { get; init; } = null!;

    public Dictionary<string, string> Tags { get; init; } = null!;
}