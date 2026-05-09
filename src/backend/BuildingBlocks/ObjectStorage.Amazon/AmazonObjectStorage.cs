using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using ObjectStorage.Abstracts;
using ObjectStorage.Abstracts.Models;
using Observability.Abstracts;
using Result;
using DeleteObjectRequest = ObjectStorage.Abstracts.Models.DeleteObjectRequest;
using GetObjectRequest = ObjectStorage.Abstracts.Models.GetObjectRequest;
using GetObjectResponse = ObjectStorage.Abstracts.Models.GetObjectResponse;

namespace ObjectStorage.Amazon;

public class AmazonObjectStorage : IObjectStorage
{
    private readonly ITracer<AmazonObjectStorage> _tracer;
    private readonly ILogger<AmazonObjectStorage> _logger;
    private readonly IAmazonS3 _amazonS3;
    
    public AmazonObjectStorage(
        ITracer<AmazonObjectStorage> tracer,
        ILogger<AmazonObjectStorage> logger,
        IAmazonS3 amazonS3)
    {
        _tracer = tracer;
        _logger = logger;
        _amazonS3 = amazonS3;
    }

    public async Task<AppResult> CreateObjectAsync(CreateObjectRequest request, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Create object in S3");
        
        _logger.LogInformation("Create object in S3 Bucket: {BucketName} Key: {ObjectKey}", 
            request.BucketName, request.Key);
        
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
        
        if (createResult.HttpStatusCode is not HttpStatusCode.OK)
        {
            _logger.LogWarning("Create object in S3 failed StatusCode: {HttpStatusCode}", createResult.HttpStatusCode);
            
            return AppResult.Failure(AppErrorType.Failed, "Failed to create object");
        }
        
        return AppResult.Success();
    }

    public async Task<AppResult<GetObjectResponse>> GetObjectAsync(GetObjectRequest request, 
        CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Get object from S3");

        _logger.LogInformation("Get object from S3 Bucket: {BucketName} Key: {ObjectKey}", 
            request.BucketName, request.Key);
        
        var objectResult = await _amazonS3.GetObjectAsync(
            new global::Amazon.S3.Model.GetObjectRequest
            {
                BucketName = request.BucketName,
                Key        = request.Key,
            },
            ct);
        
        if (objectResult.HttpStatusCode is not HttpStatusCode.OK)
        {
            _logger.LogWarning("Get object from S3 failed StatusCode: {HttpStatusCode}", objectResult.HttpStatusCode);
            
            return AppResult<GetObjectResponse>.Failure(AppErrorType.Failed, "Failed to get object");
        }
        
        return AppResult<GetObjectResponse>.Success(
            new GetObjectResponse
            {
                Key = objectResult.Key,
            });
    }

    public async Task<AppResult> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Delete object from S3");
        
        _logger.LogInformation("Delete object from S3 Bucket: {BucketName} Key: {ObjectKey}", 
            request.BucketName, request.Key);
        
        var deleteResult = await _amazonS3.DeleteObjectAsync(
            new global::Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = request.BucketName,
                Key        = request.Key,
            },
            ct);

        if (deleteResult.HttpStatusCode is not HttpStatusCode.NoContent)
        {
            _logger.LogWarning("Delete object from S3 failed StatusCode: {HttpStatusCode}", deleteResult.HttpStatusCode);
            
            return AppResult.Failure(AppErrorType.Failed, "Failed to delete object");
        }
        
        return AppResult.Success();
    }

    public void Dispose()
    {
        _amazonS3.Dispose();
        
        GC.SuppressFinalize(this);
    }
}