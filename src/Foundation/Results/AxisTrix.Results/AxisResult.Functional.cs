namespace AxisTrix;

public abstract partial class AxisResult
{
    #region Functional

    public AxisResult<TNew> Map<TNew>(Func<TNew> mapper) => IsSuccess ? Ok(mapper()) : PropagateErrors<TNew>(this);
    public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<Task<TNew>> mapper) => IsSuccess ? Ok(await mapper()) : PropagateErrors<TNew>(this);

    public AxisResult Then(Func<AxisResult> next) => IsSuccess ? next() : this;
    public AxisResult<TNew> Then<TNew>(Func<AxisResult<TNew>> next) => IsSuccess ? next() : PropagateErrors<TNew>(this);
    public async Task<AxisResult> ThenAsync(Func<Task<AxisResult>> next) => IsSuccess ? await next() : this;
    public async Task<AxisResult<TNew>> ThenAsync<TNew>(Func<Task<AxisResult<TNew>>> next) => IsSuccess ? await next() : PropagateErrors<TNew>(this);

    public AxisResult Tap(Action action) { if (IsSuccess) action(); return this; }
    public async Task<AxisResult> TapAsync(Func<Task> action) { if (IsSuccess) await action(); return this; }
    public virtual AxisResult TapError(Action<IReadOnlyList<AxisError>> action) { if (IsFailure) action(Errors); return this; }
    public virtual async Task<AxisResult> TapErrorAsync(Func<IReadOnlyList<AxisError>, Task> action) { if (IsFailure) await action(Errors); return this; }

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure) => IsSuccess ? onSuccess() : onFailure(Errors);
    public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure) => IsSuccess ? await onSuccess() : await onFailure(Errors);

    public virtual AxisResult MapError(Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
        => IsSuccess ? this : Error(mapper(Errors));
    public AxisResult MapError(Func<AxisError, AxisError> mapper)
        => MapError(errors => errors.Select(mapper));
    public virtual async Task<AxisResult> MapErrorAsync(Func<IReadOnlyList<AxisError>, Task<IEnumerable<AxisError>>> mapper)
        => IsSuccess ? this : Error(await mapper(Errors));

    public AxisResult OrElse(Func<IReadOnlyList<AxisError>, AxisResult> fallback)
        => IsSuccess ? this : fallback(Errors);
    public async Task<AxisResult> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult>> fallback)
        => IsSuccess ? this : await fallback(Errors);

    public AxisResult OrElse(Func<IReadOnlyList<AxisError>, AxisResult> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error(Errors.Concat(alt.Errors)) : alt;
    }
    public async Task<AxisResult> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult>> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = await fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error(Errors.Concat(alt.Errors)) : alt;
    }

    public AxisResult RequireNotFound(AxisError errorIfFound)
        => IsSuccess
            ? Error(errorIfFound)
            : Errors.All(e => e.Type == AxisErrorType.NotFound) ? Ok() : this;

    #endregion
}

public abstract partial class AxisResult<TValue>
{
    #region Functional

    public AxisResult<TNew> Map<TNew>(Func<TValue, TNew> mapper) => IsSuccess ? Ok(mapper(Value)) : PropagateErrors<TNew>(this);
    public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, Task<TNew>> mapper) => IsSuccess ? Ok(await mapper(Value)) : PropagateErrors<TNew>(this);

    public AxisResult Then(Func<TValue, AxisResult> next) => IsSuccess ? next(Value) : this;
    public AxisResult<TNew> Then<TNew>(Func<TValue, AxisResult<TNew>> next) => IsSuccess ? next(Value) : PropagateErrors<TNew>(this);
    public async Task<AxisResult> ThenAsync(Func<TValue, Task<AxisResult>> next) => IsSuccess ? await next(Value) : this;
    public async Task<AxisResult<TNew>> ThenAsync<TNew>(Func<TValue, Task<AxisResult<TNew>>> next) => IsSuccess ? await next(Value) : PropagateErrors<TNew>(this);

    public new AxisResult<TValue> Tap(Action action) { if (IsSuccess) action(); return this; }
    public AxisResult<TValue> Tap(Action<TValue> action) { if (IsSuccess) action(Value); return this; }
    public new async Task<AxisResult<TValue>> TapAsync(Func<Task> action) { if (IsSuccess) await action(); return this; }
    public async Task<AxisResult<TValue>> TapAsync(Func<TValue, Task> action) { if (IsSuccess) await action(Value); return this; }
    public override AxisResult<TValue> TapError(Action<IReadOnlyList<AxisError>> action) { if (IsFailure) action(Errors); return this; }
    public new async Task<AxisResult<TValue>> TapErrorAsync(Func<IReadOnlyList<AxisError>, Task> action) { if (IsFailure) await action(Errors); return this; }

    public AxisResult<TValue> Ensure(Func<TValue, bool> predicate, AxisError error) => !IsSuccess ? this : (predicate(Value) ? this : Error<TValue>(error));
    public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, Task<bool>> predicate, AxisError error) => !IsSuccess ? this : (await predicate(Value) ? this : Error<TValue>(error));
    public AxisResult<TValue> Ensure(Func<TValue, AxisResult> validation)
    {
        if (!IsSuccess) return this;
        var validationResult = validation(Value);
        return validationResult.IsSuccess ? this : PropagateErrors<TValue>(validationResult);
    }
    public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, Task<AxisResult>> validation)
    {
        if (!IsSuccess) return this;
        var result = await validation(Value);
        return result.IsSuccess ? this : PropagateErrors<TValue>(result);
    }

    public AxisResult<(TValue Value1, TNew Value2)> Zip<TNew>(Func<TValue, TNew> mapper)
        => IsSuccess ? Ok<(TValue Value1, TNew Value2)>((Value, mapper(Value))) : PropagateErrors<(TValue Value1, TNew Value2)>(this);
    public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, Task<TNew>> mapper)
        => IsSuccess ? Ok<(TValue Value1, TNew Value2)>((Value, await mapper(Value))) : PropagateErrors<(TValue Value1, TNew Value2)>(this);
    public AxisResult<(TValue Value1, TNew Value2)> Zip<TNew>(Func<TValue, AxisResult<TNew>> mapper)
    {
        if (!IsSuccess) return PropagateErrors<(TValue Value1, TNew Value2)>(this);
        var r = mapper(Value);
        return r.IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, r.Value))
            : PropagateErrors<(TValue Value1, TNew Value2)>(r);
    }
    public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, Task<AxisResult<TNew>>> mapper)
    {
        if (!IsSuccess) return PropagateErrors<(TValue Value1, TNew Value2)>(this);
        var r = await mapper(Value);
        return r.IsSuccess
            ? Ok<(TValue Value1, TNew Value2)>((Value, r.Value))
            : PropagateErrors<(TValue Value1, TNew Value2)>(r);
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure) => IsSuccess ? onSuccess(Value) : onFailure(Errors);
    public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure) => IsSuccess ? await onSuccess(Value) : await onFailure(Errors);

    public override AxisResult<TValue> MapError(Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
        => IsSuccess ? this : Error<TValue>(mapper(Errors));
    public new AxisResult<TValue> MapError(Func<AxisError, AxisError> mapper)
        => MapError(errors => errors.Select(mapper));
    public new async Task<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, Task<IEnumerable<AxisError>>> mapper)
        => IsSuccess ? this : Error<TValue>(await mapper(Errors));

    public AxisResult<TValue> Recover(Func<IReadOnlyList<AxisError>, TValue> recovery)
        => IsSuccess ? this : Ok(recovery(Errors));
    public async Task<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
        => IsSuccess ? this : Ok(await recovery(Errors));
    public AxisResult<TValue> Recover(Func<TValue> recovery)
        => IsSuccess ? this : Ok(recovery());
    public async Task<AxisResult<TValue>> RecoverAsync(Func<Task<TValue>> recovery)
        => IsSuccess ? this : Ok(await recovery());
    public AxisResult<TValue> Recover(TValue defaultValue)
        => IsSuccess ? this : Ok(defaultValue);

    public AxisResult<TValue> RecoverWhen(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, TValue> recovery)
        => IsSuccess ? this : (predicate(Errors) ? Ok(recovery(Errors)) : this);
    public async Task<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
        => IsSuccess ? this : (predicate(Errors) ? Ok(await recovery(Errors)) : this);
    public AxisResult<TValue> RecoverWhen(AxisErrorType type, Func<TValue> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Type == type) ? Ok(recovery()) : this);
    public async Task<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<Task<TValue>> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Type == type) ? Ok(await recovery()) : this);
    public AxisResult<TValue> RecoverWhen(string code, Func<TValue> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Code == code) ? Ok(recovery()) : this);
    public async Task<AxisResult<TValue>> RecoverWhenAsync(string code, Func<Task<TValue>> recovery)
        => IsSuccess ? this : (Errors.Any(e => e.Code == code) ? Ok(await recovery()) : this);

    public AxisResult<TValue> RecoverNotFound(Func<TValue> recovery)
        => IsSuccess ? this : (Errors.All(e => e.Type == AxisErrorType.NotFound) ? Ok(recovery()) : this);
    public async Task<AxisResult<TValue>> RecoverNotFoundAsync(Func<Task<TValue>> recovery)
        => IsSuccess ? this : (Errors.All(e => e.Type == AxisErrorType.NotFound) ? Ok(await recovery()) : this);

    public AxisResult<TValue> OrElse(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback)
        => IsSuccess ? this : fallback(Errors);
    public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback)
        => IsSuccess ? this : await fallback(Errors);
    public AxisResult<TValue> OrElse(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error<TValue>(Errors.Concat(alt.Errors)) : alt;
    }
    public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback, bool combineErrors)
    {
        if (IsSuccess) return this;
        var alt = await fallback(Errors);
        if (alt.IsSuccess) return alt;
        return combineErrors ? Error<TValue>(Errors.Concat(alt.Errors)) : alt;
    }

    // LINQ query syntax support
    public AxisResult<TNew> Select<TNew>(Func<TValue, TNew> selector) => Map(selector);
    public AxisResult<TNew> SelectMany<TIntermediate, TNew>(
        Func<TValue, AxisResult<TIntermediate>> binder,
        Func<TValue, TIntermediate, TNew> projector)
    {
        if (!IsSuccess) return PropagateErrors<TNew>(this);
        var inner = binder(Value);
        return inner.IsSuccess
            ? Ok(projector(Value, inner.Value))
            : PropagateErrors<TNew>(inner);
    }
    public async Task<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
        Func<TValue, Task<AxisResult<TIntermediate>>> binder,
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
