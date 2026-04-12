using AxisResult;
using FluentValidation;

namespace AxisTrix.Validation.FluentValidator;

public class FluentValidatorAdapter<T>(IAxisMediator mediator, IValidator<T> validator) : IAxisValidator<T>
{
    public AxisResult.AxisResult Validate(T instance)
    {
        var result = validator.Validate(instance);

        if (result.IsValid)
            return AxisResult.AxisResult.Ok();

        var errors = result.Errors
            .Select(e => AxisError.ValidationRule(e.ErrorCode))
            .ToList();

        return AxisResult.AxisResult.Error(errors);
    }

    public async Task<AxisResult.AxisResult> ValidateAsync(T instance)
    {
        var result = await validator.ValidateAsync(instance, mediator.CancellationToken);

        if (result.IsValid)
            return AxisResult.AxisResult.Ok();

        var errors = result.Errors
            .Select(e => AxisError.ValidationRule(e.ErrorCode))
            .ToList();

        return AxisResult.AxisResult.Error(errors);
    }
}
