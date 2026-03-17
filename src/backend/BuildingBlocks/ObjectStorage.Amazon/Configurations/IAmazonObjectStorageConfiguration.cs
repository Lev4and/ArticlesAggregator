namespace ObjectStorage.Amazon.Configurations;

public interface IAmazonObjectStorageConfiguration
{
    string Host { get; }
    
    int Port { get; }
}