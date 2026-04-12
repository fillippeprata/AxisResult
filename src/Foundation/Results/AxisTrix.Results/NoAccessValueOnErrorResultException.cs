namespace AxisTrix;

public sealed class NoAccessValueOnErrorResultException(IReadOnlyList<AxisError> errors) : InvalidOperationException(BuildMessage(errors))
{
    public IReadOnlyList<AxisError> Errors { get; } = errors;

    private static string BuildMessage(IReadOnlyList<AxisError> errors)
    {
        var codes = string.Join(", ", errors.Select(e => e.Code));
        return $"Cannot access Value on a failed AxisResult. The result contains {errors.Count} error(s): {codes}";
    }
}
