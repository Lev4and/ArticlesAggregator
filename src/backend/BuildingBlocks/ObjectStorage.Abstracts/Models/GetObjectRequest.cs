namespace ObjectStorage.Abstracts.Models;

public record GetObjectRequest
{
    public string BucketName { get; init; } = null!;

    public string Key { get; init; } = null!;
}