using System.Diagnostics;

namespace AxisResult;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract partial class AxisResult(List<AxisError>? initErrors = null)
{
    protected readonly List<AxisError>? _errors = initErrors is { Count: > 0 } ? initErrors : null;

    public IReadOnlyList<AxisError> Errors => _errors ?? (IReadOnlyList<AxisError>)Array.Empty<AxisError>();

    protected List<AxisError>? RawErrors => _errors;

    public string JoinErrorCodes(string separator = ", ")
    {
        if (_errors == null || _errors.Count == 0) return string.Empty;
        return string.Join(separator, _errors.Select(e => e.Code));
    }

    public bool IsSuccess => _errors == null || _errors.Count == 0;
    public bool IsFailure => !IsSuccess;

    public void Deconstruct(out bool isSuccess, out IReadOnlyList<AxisError> errors)
    {
        isSuccess = IsSuccess;
        errors = Errors;
    }

    internal virtual string DebuggerDisplay =>
        IsSuccess ? "Ok" : $"Error[{_errors!.Count}]: {JoinErrorCodes()}";

    public static implicit operator AxisResult(AxisError error) => Error(error);
    public static implicit operator AxisResult(List<AxisError> errors) => Error(errors);
    public static implicit operator AxisResult(AxisError[] errors) => Error(errors);
}
