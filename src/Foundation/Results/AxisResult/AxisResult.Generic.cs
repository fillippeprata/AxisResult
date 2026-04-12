namespace AxisResult;

public abstract partial class AxisResult<TValue>(TValue? value, List<AxisError>? errors = null) : AxisResult(errors)
{
    protected readonly TValue? _value = value;

    public virtual TValue Value => IsSuccess
        ? _value!
        : throw new NoAccessValueOnErrorResultException(Errors);

    public static implicit operator AxisResult<TValue>(TValue value) => Ok(value);
    public static implicit operator AxisResult<TValue>(AxisError error) => Error<TValue>(error);
    public static implicit operator AxisResult<TValue>(List<AxisError> errors) => Error<TValue>(errors);
    public static implicit operator AxisResult<TValue>(AxisError[] errors) => Error<TValue>(errors);
}
