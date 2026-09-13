using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError($"Error Message: {exception.Message}, Time of occurence {DateTime.UtcNow} Utc.");

        var problemDetails = GetProblemDetails(context, exception);
        
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails GetProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails();

        problemDetails.Instance = context.Request.Path;
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        if (exception is ValidationException validationException)
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

        (problemDetails.Detail, problemDetails.Title, problemDetails.Status) = exception switch
        {
            BadRequestException or 
            BadHttpRequestException or 
            ArgumentOutOfRangeException => (exception.Message, exception.GetType().Name, StatusCodes.Status400BadRequest),
            InternalServerException => (exception.Message, exception.GetType().Name, StatusCodes.Status500InternalServerError),
            ValidationException => (exception.Message, exception.GetType().Name, StatusCodes.Status422UnprocessableEntity),
            NotFoundException => (exception.Message, exception.GetType().Name, StatusCodes.Status404NotFound),
            _ => (exception.Message, exception.GetType().Name, StatusCodes.Status500InternalServerError)
        };

        return problemDetails;
    } 
}
