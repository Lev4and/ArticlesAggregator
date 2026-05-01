namespace ObjectStorage.Amazon.Configurations;

public class AmazonObjectStorageConfiguration : IAmazonObjectStorageConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string S3_URL = nameof(S3_URL);
    private const string S3_ACCESS_KEY = nameof(S3_ACCESS_KEY);
    private const string S3_SECRET_KEY = nameof(S3_SECRET_KEY);
    private const string S3_BUCKET_NAME = nameof(S3_BUCKET_NAME);
    
    // ReSharper restore InconsistentNaming
    
    public Uri Url => new Uri(Environment.GetEnvironmentVariable(S3_URL) ?? "http://localhost:9000");

    public string AccessKey => Environment.GetEnvironmentVariable(S3_ACCESS_KEY) ?? string.Empty;
    
    public string SecretKey => Environment.GetEnvironmentVariable(S3_SECRET_KEY) ?? string.Empty;
    
    public string BucketName => Environment.GetEnvironmentVariable(S3_BUCKET_NAME) ?? "default";
}