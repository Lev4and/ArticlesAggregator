using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using ObjectStorage.Abstracts;
using ObjectStorage.Abstracts.Models;
using Result;
using DeleteObjectRequest = ObjectStorage.Abstracts.Models.DeleteObjectRequest;
using GetObjectRequest = ObjectStorage.Abstracts.Models.GetObjectRequest;
using GetObjectResponse = ObjectStorage.Abstracts.Models.GetObjectResponse;

namespace ObjectStorage.Amazon;

public class AmazonObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _amazonS3;
    
    public AmazonObjectStorage(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }

    public async Task<AppResult> CreateObjectAsync(CreateObjectRequest request, CancellationToken ct = default)
    {
        var createResult = await _amazonS3.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName  = request.BucketName,
                Key         = request.Key,
                ContentType = request.ContentType,
                InputStream = request.Stream,
                TagSet      = request.Tags.Select(tag => new Tag { Key = tag.Key, Value = tag.Value }).ToList()
            },
            ct);
        
        return createResult.HttpStatusCode is not HttpStatusCode.OK
            ? AppResult.Failure(AppErrorType.Failed, "Failed to create object")
            : AppResult.Success();
    }

    public async Task<AppResult<GetObjectResponse>> GetObjectAsync(GetObjectRequest request, 
        CancellationToken ct = default)
    {
        var objectResult = await _amazonS3.GetObjectAsync(
            new global::Amazon.S3.Model.GetObjectRequest
            {
                BucketName = request.BucketName,
                Key        = request.Key,
            },
            ct);
        
        return objectResult.HttpStatusCode is not HttpStatusCode.OK
            ? AppResult<GetObjectResponse>.Failure(AppErrorType.Failed, "Failed to get object")
            : AppResult<GetObjectResponse>.Success(
                new GetObjectResponse
                {
                    Key = objectResult.Key,
                });
    }

    public async Task<AppResult> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default)
    {
        var deleteResult = await _amazonS3.DeleteObjectAsync(
            new global::Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = request.BucketName,
                Key        = request.Key,
            },
            ct);
        
        return deleteResult.HttpStatusCode is not HttpStatusCode.OK
            ? AppResult.Failure(AppErrorType.Failed, "Failed to delete object")
            : AppResult.Success();
    }

    public void Dispose()
    {
        _amazonS3.Dispose();
        
        GC.SuppressFinalize(this);
    }
}