using Grpc.Core;
using Result;

namespace ProxyServerSearcher.Presentation.Extensions;

public static class AppErrorExtensions
{
    public static RpcException ToRpcException(this AppError error)
    {
        return error.Type switch
        {
            AppErrorType.Validation => 
                new RpcException(
                    new Status(
                        StatusCode.InvalidArgument, 
                        error.Message)),
            AppErrorType.Unauthorized => 
                new RpcException(
                    new Status(
                        StatusCode.Unauthenticated, 
                        error.Message)),
            AppErrorType.Forbidden => 
                new RpcException(
                    new Status(
                        StatusCode.PermissionDenied, 
                        error.Message)),
            AppErrorType.NotFound => 
                new RpcException(
                    new Status(
                        StatusCode.NotFound, 
                        error.Message)),
            AppErrorType.Conflict => 
                new RpcException(
                    new Status(
                        StatusCode.AlreadyExists, 
                        error.Message)),
            _ => new RpcException(
                new Status(
                    StatusCode.Internal, 
                    error.Message)),
        };
    }
}