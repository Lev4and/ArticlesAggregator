using Grpc.Protos;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;

namespace ProxyServerAggregator.Presentation.Grpc;

public class ProxyServerAggregatorGrpcServiceMapper
{
    public static ProxyServerRpc MapToRpc(ProxyServerDto proxyServer)
    {
        return new ProxyServerRpc
        {
            Id                = proxyServer.Id.ToString(),
            Protocol          = (ProxyServerProtocolRpc)proxyServer.Protocol,
            HostnameOrAddress = proxyServer.HostnameOrAddress,
            Port              = proxyServer.Port,
            Credentials       = proxyServer.Credentials is not null
                ? new ProxyServerCredentialsRpc
                {
                    Username = proxyServer.Credentials.Username, 
                    Password = proxyServer.Credentials.Password
                }
                : null,
        };
    }
}