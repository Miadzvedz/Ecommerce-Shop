namespace BuildingBlocks.Behaviors;

public class LoggingBenavior<TReqest, TResponse> : IPipelineBehavior<TReqest, TResponse>
    where TReqest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBenavior<TReqest, TResponse>> _logger;
    private readonly int warningThresholdTime = 3;

    public LoggingBenavior(ILogger<LoggingBenavior<TReqest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TReqest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[START] Handle request={typeof(TReqest).Name} " +
            $"- Response={typeof(TResponse).Name} " +
            $"- Request data = {request}.");

        var time = new Stopwatch();
        time.Start();

        var response = await next.Invoke();

        time.Stop();
        var timeTaken = time.Elapsed;

        if (timeTaken.Seconds > warningThresholdTime)
            _logger.LogWarning($"[PERFOMANCE] The request {typeof(TReqest).Name} took {timeTaken.Seconds} sekonds.");

        _logger.LogInformation($"[END] Handle {typeof(TReqest).Name} with {typeof(TResponse).Name} ");

        return response;
    }
}
