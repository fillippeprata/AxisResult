namespace AxisTrix.Validation;

public interface IAxisValidator<in T>
{
    AxisResult Validate(T instance);
    Task<AxisResult> ValidateAsync(T instance);
}
