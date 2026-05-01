namespace ObjectStorage.Abstracts.Models;

public record DeleteObjectRequest
{
    public string BucketName { get; init; } = null!;

    public string Key { get; init; } = null!;
}