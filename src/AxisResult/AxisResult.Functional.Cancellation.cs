namespace Axis;

// CancellationToken-aware overloads for the generic AxisResult<TValue> pipeline.
// These are additive overloads: the existing CT-less signatures are preserved
// for retrocompatibility with consumers that close over the token via lambda.
public abstract partial class AxisResult<TValue>
{
    #region Task + CancellationToken

    public async Task<AxisResult<TNew>> MapAsync<TNew>(
        Func<TValue, CancellationToken, Task<TNew>> mapper,
        CancellationToken ct = default)
        => IsSuccess ? Ok(await mapper(Value, ct)) : PropagateErrors<TNew>(this);

    public async Task<AxisResult<TValue>> ThenAsync(
        Func<TValue, CancellationToken, Task<AxisResult>> next,
        CancellationToken ct = default)
    {
        if (IsFailure) return this;
        var nextResult = await next(Value, ct);
        return nextResult.IsSuccess ? this : PropagateErrors<TValue>(nextResult);
    }

    public async Task<AxisResult<TNew>> ThenAsync<TNew>(
        Func<TValue, CancellationToken, Task<AxisResult<TNew>>> next,
        CancellationToken ct = default)
        => IsSuccess ? await next(Value, ct) : PropagateErrors<TNew>(this);

    public async Task<AxisResult<TValue>> TapAsync(
        Func<TValue, CancellationToken, Task> action,
        CancellationToken ct = default)
    {
        if (IsSuccess) await action(Value, ct);
        return this;
    }

    public async Task<AxisResult<TValue>> EnsureAsync(
        Func<TValue, CancellationToken, Task<bool>> predicate,
        AxisError error,
        CancellationToken ct = default)
        => !IsSuccess ? this : (await predicate(Value, ct) ? this : Error<TValue>(error));

    public async Task<AxisResult<TValue>> EnsureAsync(
        Func<TValue, CancellationToken, Task<AxisResult>> validation,
        CancellationToken ct = default)
    {
        if (!IsSuccess) return this;
        var r = await validation(Value, ct);
        return r.IsSuccess ? this : PropagateErrors<TValue>(r);
    }

    public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(
        Func<TValue, CancellationToken, Task<TNew>> mapper,
        CancellationToken ct = default)
        => IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, await mapper(Value, ct)))
            : PropagateErrors<(TValue Value1, TNew Value2)>(this);

    public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(
        Func<TValue, CancellationToken, Task<AxisResult<TNew>>> mapper,
        CancellationToken ct = default)
    {
        if (!IsSuccess) return PropagateErrors<(TValue Value1, TNew Value2)>(this);
        var r = await mapper(Value, ct);
        return r.IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, r.Value))
            : PropagateErrors<(TValue Value1, TNew Value2)>(r);
    }

    #endregion

    #region ValueTask + CancellationToken

    public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(
        Func<TValue, CancellationToken, ValueTask<TNew>> mapper,
        CancellationToken ct = default)
        => IsSuccess ? Ok(await mapper(Value, ct)) : PropagateErrors<TNew>(this);

    public async ValueTask<AxisResult<TValue>> ThenAsync(
        Func<TValue, CancellationToken, ValueTask<AxisResult>> next,
        CancellationToken ct = default)
    {
        if (IsFailure) return this;
        var nextResult = await next(Value, ct);
        return nextResult.IsSuccess ? this : PropagateErrors<TValue>(nextResult);
    }

    public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(
        Func<TValue, CancellationToken, ValueTask<AxisResult<TNew>>> next,
        CancellationToken ct = default)
        => IsSuccess ? await next(Value, ct) : PropagateErrors<TNew>(this);

    public async ValueTask<AxisResult<TValue>> TapAsync(
        Func<TValue, CancellationToken, ValueTask> action,
        CancellationToken ct = default)
    {
        if (IsSuccess) await action(Value, ct);
        return this;
    }

    public async ValueTask<AxisResult<TValue>> EnsureAsync(
        Func<TValue, CancellationToken, ValueTask<bool>> predicate,
        AxisError error,
        CancellationToken ct = default)
        => !IsSuccess ? this : (await predicate(Value, ct) ? this : Error<TValue>(error));

    public async ValueTask<AxisResult<TValue>> EnsureAsync(
        Func<TValue, CancellationToken, ValueTask<AxisResult>> validation,
        CancellationToken ct = default)
    {
        if (!IsSuccess) return this;
        var r = await validation(Value, ct);
        return r.IsSuccess ? this : PropagateErrors<TValue>(r);
    }

    public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(
        Func<TValue, CancellationToken, ValueTask<TNew>> mapper,
        CancellationToken ct = default)
        => IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, await mapper(Value, ct)))
            : PropagateErrors<(TValue Value1, TNew Value2)>(this);

    public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(
        Func<TValue, CancellationToken, ValueTask<AxisResult<TNew>>> mapper,
        CancellationToken ct = default)
    {
        if (!IsSuccess) return PropagateErrors<(TValue Value1, TNew Value2)>(this);
        var r = await mapper(Value, ct);
        return r.IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, r.Value))
            : PropagateErrors<(TValue Value1, TNew Value2)>(r);
    }

    #endregion
}
