namespace ObjectStorage.Amazon.Configurations;

public interface IAmazonObjectStorageConfiguration
{
    Uri Url { get; }
    
    string AccessKey { get; }
    
    string SecretKey { get; }
    
    string BucketName { get; }
}