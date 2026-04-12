using AxisResult;
using AxisTrix.CQRS;
using AxisTrix.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Pipelines;

public class ValidationBehavior<TRequest>(IServiceProvider serviceProvider)
    : IAxisPipelineBehavior<TRequest>
    where TRequest : IAxisRequest
{
    public async Task<AxisResult.AxisResult> HandleAsync(TRequest request, AxisPipelineContext context, Func<Task<AxisResult.AxisResult>> next)
    {
        var validator = serviceProvider.GetService<IAxisValidator<TRequest>>();
        if (validator is null) return await next();

        var result = await validator.ValidateAsync(request);
        return result.IsFailure ? result : await next();
    }
}

public class ValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
    : IAxisPipelineBehavior<TRequest, TResponse>
    where TRequest : IAxisRequest
    where TResponse : IAxisResponse
{
    public async Task<AxisResult<TResponse>> HandleAsync(TRequest request, AxisPipelineContext context, Func<Task<AxisResult<TResponse>>> next)
    {
        var validator = serviceProvider.GetService<IAxisValidator<TRequest>>();
        if (validator is null) return await next();

        var result = await validator.ValidateAsync(request);
        return result.IsFailure ? result.Errors.ToArray() : await next();
    }
}
