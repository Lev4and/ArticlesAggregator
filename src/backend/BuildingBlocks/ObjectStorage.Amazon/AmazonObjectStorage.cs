using Amazon.S3;
using ObjectStorage.Abstracts;

namespace ObjectStorage.Amazon;

public class AmazonObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _amazonS3;
    
    public AmazonObjectStorage(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }
}