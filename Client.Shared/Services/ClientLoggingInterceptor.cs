using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Client.Shared.Services;

public class ClientLoggingInterceptor(ILoggerFactory loggerFactory) : Interceptor
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<ClientLoggingInterceptor>();

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Sending {request}");
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> task)
    {
        try
        {
            var response = await task;
            _logger.LogInformation($"Received {response}");
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError($"gRPC Error: {ex.Status.Detail}");
            throw;
        }
    }
}