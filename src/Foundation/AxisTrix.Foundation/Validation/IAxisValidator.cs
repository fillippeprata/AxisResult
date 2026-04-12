namespace AxisTrix.Validation;

public interface IAxisValidator<in T>
{
    AxisResult.AxisResult Validate(T instance);
    Task<AxisResult.AxisResult> ValidateAsync(T instance);
}
