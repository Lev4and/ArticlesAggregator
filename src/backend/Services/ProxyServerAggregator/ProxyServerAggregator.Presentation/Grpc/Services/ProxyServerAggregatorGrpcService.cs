using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Extensions;
using Grpc.Protos;
using Mediator;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.UseCases.ProxyServers.Queries.Get;

namespace ProxyServerAggregator.Presentation.Grpc.Services;

public class ProxyServerAggregatorGrpcService : global::Grpc.Protos.ProxyServerAggregator.ProxyServerAggregatorBase
{
    private readonly ITracer<ProxyServerAggregatorGrpcService> _tracer;
    private readonly ILogger<ProxyServerAggregatorGrpcService> _logger;
    private readonly IMediator _mediator;
    
    public ProxyServerAggregatorGrpcService(
        ITracer<ProxyServerAggregatorGrpcService> tracer, 
        ILogger<ProxyServerAggregatorGrpcService> logger, 
        IMediator mediator)
    {
        _tracer = tracer;
        _logger = logger;
        _mediator = mediator;
    }
    
    public override async Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return await Task.FromResult(new Empty());
    }

    public override async Task<ProxyServerRpc> GetProxyServer(GetProxyServerRequest request, ServerCallContext context)
    {
        using var operation = _tracer.StartOperation("Get proxy server (gRPC)");
        
        _logger.LogInformation("Get proxy server (gRPC)");

        if (!Guid.TryParse(request.Id, out var proxyServerId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid proxy server id"));
        }

        var proxyServerResult = await _mediator.Send(new GetProxyServerQuery(proxyServerId), context.CancellationToken);
        if (proxyServerResult.IsSuccess)
        {
            return ProxyServerAggregatorGrpcServiceMapper.MapToRpc(proxyServerResult.Result!);
        }

        var error = proxyServerResult.Errors.First();

        throw error.ToRpcException();
    }
}