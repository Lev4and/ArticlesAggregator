namespace Server.Kestrel.Configurations;

public interface IKestrelServerConfiguration
{
    int HttpPort { get; }
    
    int GrpcPort { get; }
}