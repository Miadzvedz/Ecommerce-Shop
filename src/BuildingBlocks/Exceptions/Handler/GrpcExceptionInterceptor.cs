namespace BuildingBlocks.Exceptions.Handler;


public class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            // Передаем управление дальше (в MediatR / Handler)
            return await continuation(request, context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Incorrect data provided.");
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found.");
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (BadRequestException ex)
        {
            logger.LogWarning(ex, "Invalid request.");
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (InternalServerException ex)
        {
            logger.LogError(ex, "Internal server error.");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unknown error.");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}