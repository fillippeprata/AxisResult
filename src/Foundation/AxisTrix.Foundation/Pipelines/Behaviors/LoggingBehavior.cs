using AxisTrix.CQRS;
using AxisTrix.Logging;
using AxisTrix.Results;

namespace AxisTrix.Pipelines.Behaviors;

public class LoggingBehavior<TRequest>(
    IAxisLogger<TRequest> logger
) : IAxisPipelineBehavior<TRequest>
    where TRequest : IAxisRequest
{
    public async Task<AxisResult> HandleAsync(TRequest request, AxisPipelineContext context, Func<Task<AxisResult>> next)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation($"Handling {requestName}.", ("RequestName", requestName));
        return await next();
    }
}

public class LoggingBehavior<TRequest, TResponse>(
    IAxisLogger<TRequest> logger
) : IAxisPipelineBehavior<TRequest, TResponse>
    where TRequest : IAxisRequest
    where TResponse : IAxisResponse
{
    public async Task<AxisResult<TResponse>> HandleAsync(TRequest request, AxisPipelineContext context, Func<Task<AxisResult<TResponse>>> next)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation($"Handling {requestName}.", ("RequestName", requestName));
        return await next();
    }
}
