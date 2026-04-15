namespace Axis;

public static class AxisResultValueTaskExtensions
{
    public static ValueTask<AxisResult> AsValueTaskAsync(this AxisResult axisResult) => new(axisResult);

    public static ValueTask<AxisResult<TValue>> AsValueTaskAsync<TValue>(this AxisResult<TValue> axisResult) => new(axisResult);

    #region ValueTask Extensions (Railway Oriented)

    // --- ValueTask<AxisResult> extensions ---

    public static async ValueTask<AxisResult> ThenAsync(this ValueTask<AxisResult> task, Func<AxisResult> next)
        => (await task).Then(next);
    public static async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(this ValueTask<AxisResult> task, Func<AxisResult<TNew>> next)
        => (await task).Then(next);
    public static async ValueTask<AxisResult> ThenAsync(this ValueTask<AxisResult> task, Func<ValueTask<AxisResult>> next)
        => await (await task).ThenAsync(next);
    public static async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(this ValueTask<AxisResult> task, Func<ValueTask<AxisResult<TNew>>> next)
        => await (await task).ThenAsync(next);

    public static async ValueTask<AxisResult> TapAsync(this ValueTask<AxisResult> task, Action action)
        => (await task).Tap(action);
    public static async ValueTask<AxisResult> TapAsync(this ValueTask<AxisResult> task, Func<ValueTask> action)
        => await (await task).TapAsync(action);

    public static async ValueTask<AxisResult> TapErrorAsync(this ValueTask<AxisResult> task, Action<IReadOnlyList<AxisError>> action)
        => (await task).TapError(action);
    public static async ValueTask<AxisResult> TapErrorAsync(this ValueTask<AxisResult> task, Func<IReadOnlyList<AxisError>, ValueTask> action)
        => await (await task).TapErrorAsync(action);

    public static async ValueTask<TResult> MatchAsync<TResult>(this ValueTask<AxisResult> task, Func<TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
        => (await task).Match(onSuccess, onFailure);
    public static async ValueTask<TResult> MatchAsync<TResult>(this ValueTask<AxisResult> task, Func<ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
        => await (await task).MatchAsync(onSuccess, onFailure);

    public static async ValueTask<AxisResult<TNew>> WithValueAsync<TNew>(this ValueTask<AxisResult> task, TNew value)
    {
        var result = await task;
        return result.IsSuccess ? AxisResult.Ok(value) : AxisResult.Error<TNew>(result.Errors);
    }

    public static async ValueTask<AxisResult> RequireNotFoundAsync(this ValueTask<AxisResult> task, AxisError errorIfFound)
        => (await task).RequireNotFound(errorIfFound);

    // --- ValueTask<AxisResult<TValue>> extensions ---

    public static async ValueTask<AxisResult<TNew>> MapAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, TNew> mapper)
        => (await task).Map(mapper);
    public static async ValueTask<AxisResult<TNew>> MapAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<TNew>> mapper)
        => await (await task).MapAsync(mapper);

    public static async ValueTask<AxisResult<TValue>> ActionAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<AxisResult>> next)
        => await (await task).ThenAsync(next);

    public static async ValueTask<AxisResult<TValue>> ThenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, AxisResult> next)
        => (await task).Then(next);
    public static async ValueTask<AxisResult<TNew>> ThenAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, AxisResult<TNew>> next)
        => (await task).Then(next);
    public static async ValueTask<AxisResult<TValue>> ThenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<AxisResult>> next)
        => await (await task).ThenAsync(next);
    public static async ValueTask<AxisResult<TNew>> ThenAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<AxisResult<TNew>>> next)
        => await (await task).ThenAsync(next);

    public static async ValueTask<AxisResult<TValue>> TapAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Action<TValue> action)
        => (await task).Tap(action);
    public static async ValueTask<AxisResult<TValue>> TapAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask> action)
        => await (await task).TapAsync(action);

    public static async ValueTask<AxisResult<TValue>> TapErrorAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Action<IReadOnlyList<AxisError>> action)
        => (await task).TapError(action);
    public static async ValueTask<AxisResult<TValue>> TapErrorAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, ValueTask> action)
        => await (await task).TapErrorAsync(action);

    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, bool> predicate, AxisError error)
        => (await task).Ensure(predicate, error);
    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<bool>> predicate, AxisError error)
        => await (await task).EnsureAsync(predicate, error);
    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, AxisResult> validation)
        => (await task).Ensure(validation);
    public static async ValueTask<AxisResult<TValue>> EnsureAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<AxisResult>> validation)
        => await (await task).EnsureAsync(validation);

    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, TNew> mapper)
        => (await task).Zip(mapper);
    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<TNew>> mapper)
        => await (await task).ZipAsync(mapper);
    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, AxisResult<TNew>> mapper)
        => (await task).Zip(mapper);
    public static async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<AxisResult<TNew>>> mapper)
        => await (await task).ZipAsync(mapper);

    public static async ValueTask<AxisResult<TValue>> MapErrorAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
        => (await task).MapError(mapper);
    public static async ValueTask<AxisResult<TValue>> MapErrorAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<AxisError, AxisError> mapper)
        => (await task).MapError(mapper);
    public static async ValueTask<AxisResult<TValue>> MapErrorAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, ValueTask<IEnumerable<AxisError>>> mapper)
        => await (await task).MapErrorAsync(mapper);

    public static async ValueTask<AxisResult<TValue>> RecoverAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, TValue> recovery)
        => (await task).Recover(recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
        => await (await task).RecoverAsync(recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue> recovery)
        => (await task).Recover(recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<ValueTask<TValue>> recovery)
        => await (await task).RecoverAsync(recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverAsync<TValue>(this ValueTask<AxisResult<TValue>> task, TValue defaultValue)
        => (await task).Recover(defaultValue);

    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, TValue> recovery)
        => (await task).RecoverWhen(predicate, recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
        => await (await task).RecoverWhenAsync(predicate, recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, AxisErrorType type, Func<TValue> recovery)
        => (await task).RecoverWhen(type, recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, AxisErrorType type, Func<ValueTask<TValue>> recovery)
        => await (await task).RecoverWhenAsync(type, recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, string code, Func<TValue> recovery)
        => (await task).RecoverWhen(code, recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverWhenAsync<TValue>(this ValueTask<AxisResult<TValue>> task, string code, Func<ValueTask<TValue>> recovery)
        => await (await task).RecoverWhenAsync(code, recovery);

    public static async ValueTask<AxisResult<TValue>> RecoverNotFoundAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<TValue> recovery)
        => (await task).RecoverNotFound(recovery);
    public static async ValueTask<AxisResult<TValue>> RecoverNotFoundAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<ValueTask<TValue>> recovery)
        => await (await task).RecoverNotFoundAsync(recovery);

    public static async ValueTask<AxisResult<TValue>> OrElseAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback)
        => (await task).OrElse(fallback);
    public static async ValueTask<AxisResult<TValue>> OrElseAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback)
        => await (await task).OrElseAsync(fallback);
    public static async ValueTask<AxisResult<TValue>> OrElseAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback, bool combineErrors)
        => (await task).OrElse(fallback, combineErrors);
    public static async ValueTask<AxisResult<TValue>> OrElseAsync<TValue>(this ValueTask<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback, bool combineErrors)
        => await (await task).OrElseAsync(fallback, combineErrors);

    public static async ValueTask<TResult> MatchAsync<TValue, TResult>(this ValueTask<AxisResult<TValue>> task, Func<TValue, TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
        => (await task).Match(onSuccess, onFailure);
    public static async ValueTask<TResult> MatchAsync<TValue, TResult>(this ValueTask<AxisResult<TValue>> task, Func<TValue, ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
        => await (await task).MatchAsync(onSuccess, onFailure);

    public static async ValueTask<AxisResult<TNew>> SelectManyAsync<TValue, TIntermediate, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, AxisResult<TIntermediate>> binder,
        Func<TValue, TIntermediate, TNew> projector)
        => (await task).SelectMany(binder, projector);
    public static async ValueTask<AxisResult<TNew>> SelectManyAsync<TValue, TIntermediate, TNew>(
        this ValueTask<AxisResult<TValue>> task,
        Func<TValue, ValueTask<AxisResult<TIntermediate>>> binder,
        Func<TValue, TIntermediate, TNew> projector)
        => await (await task).SelectManyAsync(binder, projector);

    public static async ValueTask<AxisResult> RequireNotFoundAsync<TValue>(this ValueTask<AxisResult<TValue>> task, AxisError errorIfFound)
        => (await task).RequireNotFound(errorIfFound);

    // --- ValueTask<AxisResult<(T1, T2)>> Tuple2 extensions ---

    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2));
    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, ValueTask<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2));

    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, T3> mapper)
    {
        var r = await task;
        return r.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, mapper(r.Value.Value1, r.Value.Value2)))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, ValueTask<T3>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var v3 = await mapper(r.Value.Value1, r.Value.Value2);
        return AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, v3));
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, AxisResult<T3>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var m = mapper(r.Value.Value1, r.Value.Value2);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, ValueTask<AxisResult<T3>>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var m = await mapper(r.Value.Value1, r.Value.Value2);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
    }

    // --- ValueTask<AxisResult<(T1, T2, T3)>> Tuple3 extensions ---

    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, T3, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));
    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, T3, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, ValueTask<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, T4> mapper)
    {
        var r = await task;
        return r.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3)))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, ValueTask<T4>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var v4 = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, v4));
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, AxisResult<T4>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var m = mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
    }
    public static async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, ValueTask<AxisResult<T4>>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var m = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
    }

    // --- ValueTask<AxisResult<(T1, T2, T3, T4)>> Tuple4 extensions ---

    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, T3, T4, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task, Func<T1, T2, T3, T4, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));
    public static async ValueTask<AxisResult<TNew>> MapAsync<T1, T2, T3, T4, TNew>(this ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task, Func<T1, T2, T3, T4, ValueTask<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));

    #endregion
}
