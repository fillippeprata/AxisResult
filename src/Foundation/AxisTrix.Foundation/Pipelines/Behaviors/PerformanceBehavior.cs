using System.Diagnostics;
using AxisTrix.CQRS;
using AxisTrix.Logging;

namespace AxisTrix.Pipelines.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(IAxisLogger<TRequest> logger)
    : IAxisPipelineBehavior<TRequest, TResponse>
    where TRequest : IAxisRequest
    where TResponse : IAxisResponse
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<AxisResult<TResponse>> HandleAsync(TRequest request, AxisPipelineContext context, Func<Task<AxisResult<TResponse>>> next)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
            logger.LogWarning($"Slow request: {typeof(TRequest).Name} took {sw.ElapsedMilliseconds}ms");

        return response;
    }
}
