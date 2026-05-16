using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mediator;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;

namespace ProxyServerTester.Presentation.Grpc.Services;

public class ProxyServerTesterGrpcService : global::Grpc.Protos.ProxyServerTester.ProxyServerTesterBase
{
    private readonly ITracer<ProxyServerTesterGrpcService> _tracer;
    private readonly ILogger<ProxyServerTesterGrpcService> _logger;
    private readonly IMediator _mediator;
    
    public ProxyServerTesterGrpcService(
        ITracer<ProxyServerTesterGrpcService> tracer, 
        ILogger<ProxyServerTesterGrpcService> logger, 
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
}