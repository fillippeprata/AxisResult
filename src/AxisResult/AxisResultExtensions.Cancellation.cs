namespace Axis;

// Extensions on Task<AxisResult<TValue>> and ValueTask<AxisResult<TValue>> that
// accept CancellationToken-aware delegates. These are the primary surface users
// interact with when building fluent pipelines.
public static class AxisResultCancellationExtensions
{
    #region Task<AxisResult<TValue>> + CancellationToken

    public static async Task<AxisResult<TNew>> MapAsync<TValue, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<TNew>> mapper,
        CancellationToken ct = default)
        => await (await task).MapAsync(mapper, ct);

    public static async Task<AxisResult<TValue>> ThenAsync<TValue>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<AxisResult>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async Task<AxisResult<TNew>> ThenAsync<TValue, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<AxisResult<TNew>>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async Task<AxisResult> ToAxisResultAsync<TValue>(
    this Task<AxisResult<TValue>> task,
    Func<TValue, CancellationToken, Task<AxisResult>> next,
    CancellationToken ct = default)
    => await (await task).ToAxisResultAsync(next, ct);

    public static async Task<AxisResult<TValue>> ActionAsync<TValue>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<AxisResult>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async Task<AxisResult<TValue>> TapAsync<TValue>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task> action,
        CancellationToken ct = default)
        => await (await task).TapAsync(action, ct);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<bool>> predicate,
        AxisError error,
        CancellationToken ct = default)
        => await (await task).EnsureAsync(predicate, error, ct);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<AxisResult>> validation,
        CancellationToken ct = default)
        => await (await task).EnsureAsync(validation, ct);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<TNew>> mapper,
        CancellationToken ct = default)
        => await (await task).ZipAsync(mapper, ct);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, Task<AxisResult<TNew>>> mapper,
        CancellationToken ct = default)
        => await (await task).ZipAsync(mapper, ct);

    #endregion

    #region ValueTask<AxisResult<TValue>> + CancellationToken

    public static async ValueTask<AxisResult<TNew>> MapAsync<TValue, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<TNew>> mapper,
        CancellationToken ct = default)
        => await (await task).MapAsync(mapper, ct);

    public static async ValueTask<AxisResult<TValue>> ThenAsync<TValue>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<AxisResult>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async ValueTask<AxisResult<TNew>> ThenAsync<TValue, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<AxisResult<TNew>>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async ValueTask<AxisResult<TValue>> ActionAsync<TValue>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<AxisResult>> next,
        CancellationToken ct = default)
        => await (await task).ThenAsync(next, ct);

    public static async ValueTask<AxisResult<TValue>> TapAsync<TValue>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask> action,
        CancellationToken ct = default)
        => await (await task).TapAsync(action, ct);

    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<bool>> predicate,
        AxisError error,
        CancellationToken ct = default)
        => await (await task).EnsureAsync(predicate, error, ct);

    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<AxisResult>> validation,
        CancellationToken ct = default)
        => await (await task).EnsureAsync(validation, ct);

    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<TNew>> mapper,
        CancellationToken ct = default)
        => await (await task).ZipAsync(mapper, ct);

    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, CancellationToken, ValueTask<AxisResult<TNew>>> mapper,
        CancellationToken ct = default)
        => await (await task).ZipAsync(mapper, ct);

    #endregion
}
