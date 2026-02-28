namespace Server.Kestrel.Configurations;

public class KestrelServerConfiguration : IKestrelServerConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string HTTP_PORT = nameof(HTTP_PORT);
    private const string GRPC_PORT = nameof(GRPC_PORT);
    
    // ReSharper restore InconsistentNaming
    
    public int HttpPort => int.Parse(Environment.GetEnvironmentVariable(HTTP_PORT) 
        ?? throw new ArgumentException($"{nameof(HTTP_PORT)} environment variable is not set."));
    
    public int GrpcPort => int.Parse(Environment.GetEnvironmentVariable(GRPC_PORT) 
        ?? throw new ArgumentException($"{nameof(GRPC_PORT)} environment variable is not set."));
}