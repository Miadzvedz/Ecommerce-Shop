namespace BuildingBlocks.Exceptions.Handler;


public class GrpcExceptionInterceptor(IServiceProvider serviceProvider, ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{  
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await Validation(request, context.CancellationToken);

            return await continuation(request, context);
        }
        catch (RpcException ex) 
        {
            logger.LogWarning(ex,"{Status}. The error in method {Method}",ex.StatusCode, context.Method);

            throw;
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex,
                "Resource not found. The error in method {Method}",
                context.Method);

            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (BadRequestException ex)
        {
            logger.LogWarning(ex,
                "Invalid request. The error in method {Method}",
                context.Method);

            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (InternalServerException ex)
        {
            logger.LogError(ex,
                "Internal server error. The error in method {Method}",
                context.Method);

            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Unknown error. The error in method {Method}", 
                context.Method);

            throw new RpcException(new Status(StatusCode.Unknown, ex.Message));
        }
    }

    private async Task Validation<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        IValidator<TRequest>? validator = serviceProvider.GetService<IValidator<TRequest>>();

        if (validator is null) return;


        ValidationResult result = await validator.ValidateAsync(request, cancellationToken);


        if (!result.IsValid)
        {
            BadRequest badRequest = new BadRequest();

            foreach (ValidationFailure error in result.Errors)
            {
                badRequest.FieldViolations.Add(new BadRequest.Types.FieldViolation
                {
                    Field = error.PropertyName,        
                    Description = error.ErrorMessage   
                });
            }

            var rpcStatus = new Google.Rpc.Status
            {
                Code = (int)Code.InvalidArgument,
                Message = "Validation failed"
            };

            rpcStatus.Details.Add(Google.Protobuf.WellKnownTypes.Any.Pack(badRequest));

            throw rpcStatus.ToRpcException();
        }       
    }
}

