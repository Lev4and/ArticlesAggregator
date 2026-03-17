namespace ObjectStorage.Amazon.Configurations;

public class AmazonObjectStorageConfiguration : IAmazonObjectStorageConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string S3_HOST = nameof(S3_HOST);
    private const string S3_PORT = nameof(S3_PORT);
    
    // ReSharper restore InconsistentNaming
    
    public string Host => Environment.GetEnvironmentVariable(S3_HOST) 
        ?? throw new ArgumentException($"{nameof(S3_HOST)} environment variable is not set.");
    
    public int Port => int.Parse(Environment.GetEnvironmentVariable(S3_PORT) 
        ?? throw new ArgumentException($"{nameof(S3_PORT)} environment variable is not set."));
}