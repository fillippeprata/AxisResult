namespace AxisResult;

public abstract partial class AxisResult
{
    #region ValueTask Functional

    public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<ValueTask<TNew>> mapper)
        => IsSuccess ? Ok(await mapper()) : PropagateErrors<TNew>(this);

    public async ValueTask<AxisResult> ThenAsync(Func<ValueTask<AxisResult>> next)
        => IsSuccess ? await next() : this;
    public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<ValueTask<AxisResult<TNew>>> next)
        => IsSuccess ? await next() : PropagateErrors<TNew>(this);

    public async ValueTask<AxisResult> TapAsync(Func<ValueTask> action)
    { if (IsSuccess) await action(); return this; }

    public async ValueTask<AxisResult> TapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask> action)
    { if (IsFailure) await action(Errors); return this; }

    public async ValueTask<TResult> MatchAsync<TResult>(Func<ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
        => IsSuccess ? await onSuccess() : await onFailure(Errors);

    public async ValueTask<AxisResult> MapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask<IEnumerable<AxisError>>> mapper)
        => IsSuccess ? this : Error(await mapper(Errors));

    public async ValueTask<AxisResult> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult>> fallback)
        => IsSuccess ? this : await fallback(Errors);
    public async ValueTask<AxisResult> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult>> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = await fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error(Errors.Concat(alt.Errors)) : alt;
    }

    #endregion
}

public abstract partial class AxisResult<TValue>
{
    #region ValueTask Functional

    public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, ValueTask<TNew>> mapper)
        => IsSuccess ? Ok(await mapper(Value)) : PropagateErrors<TNew>(this);

    public async ValueTask<AxisResult<TValue>> ThenAsync(Func<TValue, ValueTask<AxisResult>> next)
    {
        if (IsFailure) return this;
        var nextResult = await next(Value);
        return nextResult.IsSuccess ? this : PropagateErrors<TValue>(nextResult);
    }
    public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<TValue, ValueTask<AxisResult<TNew>>> next)
        => IsSuccess ? await next(Value) : PropagateErrors<TNew>(this);

    public new async ValueTask<AxisResult<TValue>> TapAsync(Func<ValueTask> action)
    { if (IsSuccess) await action(); return this; }
    public async ValueTask<AxisResult<TValue>> TapAsync(Func<TValue, ValueTask> action)
    { if (IsSuccess) await action(Value); return this; }
    public new async ValueTask<AxisResult<TValue>> TapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask> action)
    { if (IsFailure) await action(Errors); return this; }

    public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, ValueTask<bool>> predicate, AxisError error)
        => !IsSuccess ? this : (await predicate(Value) ? this : Error<TValue>(error));
    public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, ValueTask<AxisResult>> validation)
    {
        if (!IsSuccess) return this;
        var r = await validation(Value);
        return r.IsSuccess ? this : PropagateErrors<TValue>(r);
    }

    public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, ValueTask<TNew>> mapper)
        => IsSuccess ? Ok<(TValue Value1, TNew Value2)>((Value, await mapper(Value))) : PropagateErrors<(TValue Value1, TNew Value2)>(this);
    public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, ValueTask<AxisResult<TNew>>> mapper)
    {
        if (!IsSuccess) return PropagateErrors<(TValue Value1, TNew Value2)>(this);
        var r = await mapper(Value);
        return r.IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, r.Value))
            : PropagateErrors<(TValue Value1, TNew Value2)>(r);
    }

    public async ValueTask<TResult> MatchAsync<TResult>(Func<TValue, ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
        => IsSuccess ? await onSuccess(Value) : await onFailure(Errors);

    public new async ValueTask<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask<IEnumerable<AxisError>>> mapper)
        => IsSuccess ? this : Error<TValue>(await mapper(Errors));

    public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
        => IsSuccess ? this : Ok(await recovery(Errors));
    public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<ValueTask<TValue>> recovery)
        => IsSuccess ? this : Ok(await recovery());

    public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
        => IsSuccess ? this : (predicate(Errors) ? Ok(await recovery(Errors)) : this);
    public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<ValueTask<TValue>> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Type == type) ? Ok(await recovery()) : this);
    public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(string code, Func<ValueTask<TValue>> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Code == code) ? Ok(await recovery()) : this);

    public async ValueTask<AxisResult<TValue>> RecoverNotFoundAsync(Func<ValueTask<TValue>> recovery)
        => IsSuccess ? this : (Errors.All(e => e.Type == AxisErrorType.NotFound) ? Ok(await recovery()) : this);

    public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback)
        => IsSuccess ? this : await fallback(Errors);
    public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = await fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error<TValue>(Errors.Concat(alt.Errors)) : alt;
    }

    public async ValueTask<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
        Func<TValue, ValueTask<AxisResult<TIntermediate>>> binder,
        Func<TValue, TIntermediate, TNew> projector)
    {
        if (!IsSuccess) return PropagateErrors<TNew>(this);
        var inner = await binder(Value);
        return inner.IsSuccess
            ? Ok(projector(Value, inner.Value))
            : PropagateErrors<TNew>(inner);
    }

    #endregion
}
