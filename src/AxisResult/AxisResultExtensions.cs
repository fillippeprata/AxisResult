namespace AxisResult;

public static class AxisResultExtensions
{
    public static Task<AxisResult> AsTaskAsync(this AxisResult axisResult) => Task.FromResult(axisResult);

    public static Task<AxisResult<TValue>> AsTaskAsync<TValue>(this AxisResult<TValue> axisResult) => Task.FromResult(axisResult);

    #region Task Extensions (Railway Oriented)

    public static async Task<AxisResult> ThenAsync(this Task<AxisResult> task, Func<AxisResult> next)
        => (await task).Then(next);

    public static async Task<AxisResult<TNew>> WithValueAsync<TNew>(this Task<AxisResult> task, TNew value)
    {
        var result = await task;
        return result.IsSuccess ? AxisResult.Ok(value) : result.Errors.ToArray();
    }

    public static async Task<AxisResult<TNew>> ThenAsync<TNew>(this Task<AxisResult> task, Func<AxisResult<TNew>> next)
        => (await task).Then(next);

    public static async Task<AxisResult> ThenAsync(this Task<AxisResult> task, Func<Task<AxisResult>> next)
        => await (await task).ThenAsync(next);

    public static async Task<AxisResult<TNew>> ThenAsync<TNew>(this Task<AxisResult> task, Func<Task<AxisResult<TNew>>> next)
        => await (await task).ThenAsync(next);

    public static async Task<AxisResult> TapAsync(this Task<AxisResult> task, Action action)
        => (await task).Tap(action);

    public static async Task<AxisResult> TapAsync(this Task<AxisResult> task, Func<Task> action)
        => await (await task).TapAsync(action);

    public static async Task<AxisResult> TapErrorAsync(this Task<AxisResult> task, Action<IReadOnlyList<AxisError>> action)
        => (await task).TapError(action);

    public static async Task<AxisResult> TapErrorAsync(this Task<AxisResult> task, Func<IReadOnlyList<AxisError>, Task> action)
        => await (await task).TapErrorAsync(action);

    public static async Task<TResult> MatchAsync<TResult>(this Task<AxisResult> task, Func<TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
        => (await task).Match(onSuccess, onFailure);

    public static async Task<TResult> MatchAsync<TResult>(this Task<AxisResult> task, Func<Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure)
        => await (await task).MatchAsync(onSuccess, onFailure);

    public static async Task<AxisResult> RequireNotFoundAsync(this Task<AxisResult> task, AxisError errorIfFound)
        => (await task).RequireNotFound(errorIfFound);

    // --- Task<AxisResult<TValue>> extensions ---

    public static async Task<AxisResult<TNew>> MapAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, TNew> mapper)
        => (await task).Map(mapper);

    public static async Task<AxisResult<TNew>> MapAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, Task<TNew>> mapper)
        => await (await task).MapAsync(mapper);

    public static async Task<AxisResult<TValue>> ActionAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, Task<AxisResult>> next)
        => await (await task).ThenAsync(next);

    public static async Task<AxisResult<TValue>> ThenAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, AxisResult> next)
        => (await task).Then(next);

    public static async Task<AxisResult<TValue>> ThenAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, Task<AxisResult>> next)
        => await (await task).ThenAsync(next);

    public static async Task<AxisResult<TValue>> TapAsync<TValue>(this Task<AxisResult<TValue>> task, Action<TValue> action)
        => (await task).Tap(action);

    public static async Task<AxisResult<TValue>> TapAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, Task> action)
        => await (await task).TapAsync(action);

    public static async Task<AxisResult<TValue>> TapErrorAsync<TValue>(this Task<AxisResult<TValue>> task, Action<IReadOnlyList<AxisError>> action)
        => (await task).TapError(action);

    public static async Task<AxisResult<TValue>> TapErrorAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, Task> action)
        => await (await task).TapErrorAsync(action);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, bool> predicate, AxisError error)
        => (await task).Ensure(predicate, error);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, Task<bool>> predicate, AxisError error)
        => await (await task).EnsureAsync(predicate, error);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, AxisResult> validation)
        => (await task).Ensure(validation);

    public static async Task<AxisResult<TValue>> EnsureAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue, Task<AxisResult>> validation)
        => await (await task).EnsureAsync(validation);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, TNew> mapper)
        => (await task).Zip(mapper);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, Task<TNew>> mapper)
        => await (await task).ZipAsync(mapper);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, AxisResult<TNew>> mapper)
        => (await task).Zip(mapper);

    public static async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TValue, TNew>(this Task<AxisResult<TValue>> task, Func<TValue, Task<AxisResult<TNew>>> mapper)
        => await (await task).ZipAsync(mapper);

    public static async Task<AxisResult<TValue>> MapErrorAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
        => (await task).MapError(mapper);

    public static async Task<AxisResult<TValue>> MapErrorAsync<TValue>(this Task<AxisResult<TValue>> task, Func<AxisError, AxisError> mapper)
        => (await task).MapError(mapper);

    public static async Task<AxisResult<TValue>> MapErrorAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, Task<IEnumerable<AxisError>>> mapper)
        => await (await task).MapErrorAsync(mapper);

    public static async Task<AxisResult<TValue>> RecoverAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, TValue> recovery)
        => (await task).Recover(recovery);

    public static async Task<AxisResult<TValue>> RecoverAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
        => await (await task).RecoverAsync(recovery);

    public static async Task<AxisResult<TValue>> RecoverAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue> recovery)
        => (await task).Recover(recovery);

    public static async Task<AxisResult<TValue>> RecoverAsync<TValue>(this Task<AxisResult<TValue>> task, Func<Task<TValue>> recovery)
        => await (await task).RecoverAsync(recovery);

    public static async Task<AxisResult<TValue>> RecoverAsync<TValue>(this Task<AxisResult<TValue>> task, TValue defaultValue)
        => (await task).Recover(defaultValue);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, TValue> recovery)
        => (await task).RecoverWhen(predicate, recovery);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
        => await (await task).RecoverWhenAsync(predicate, recovery);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, AxisErrorType type, Func<TValue> recovery)
        => (await task).RecoverWhen(type, recovery);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, AxisErrorType type, Func<Task<TValue>> recovery)
        => await (await task).RecoverWhenAsync(type, recovery);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, string code, Func<TValue> recovery)
        => (await task).RecoverWhen(code, recovery);

    public static async Task<AxisResult<TValue>> RecoverWhenAsync<TValue>(this Task<AxisResult<TValue>> task, string code, Func<Task<TValue>> recovery)
        => await (await task).RecoverWhenAsync(code, recovery);

    public static async Task<AxisResult<TValue>> RecoverNotFoundAsync<TValue>(this Task<AxisResult<TValue>> task, Func<TValue> recovery)
        => (await task).RecoverNotFound(recovery);

    public static async Task<AxisResult<TValue>> RecoverNotFoundAsync<TValue>(this Task<AxisResult<TValue>> task, Func<Task<TValue>> recovery)
        => await (await task).RecoverNotFoundAsync(recovery);

    public static async Task<AxisResult<TValue>> OrElseAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback)
        => (await task).OrElse(fallback);

    public static async Task<AxisResult<TValue>> OrElseAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback)
        => await (await task).OrElseAsync(fallback);

    public static async Task<AxisResult<TValue>> OrElseAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback, bool combineErrors)
        => (await task).OrElse(fallback, combineErrors);

    public static async Task<AxisResult<TValue>> OrElseAsync<TValue>(this Task<AxisResult<TValue>> task, Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback, bool combineErrors)
        => await (await task).OrElseAsync(fallback, combineErrors);

    public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<AxisResult<TValue>> task, Func<TValue, TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
        => (await task).Match(onSuccess, onFailure);

    public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<AxisResult<TValue>> task, Func<TValue, Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure)
        => await (await task).MatchAsync(onSuccess, onFailure);

    public static async Task<AxisResult<TNew>> SelectManyAsync<TValue, TIntermediate, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, AxisResult<TIntermediate>> binder,
        Func<TValue, TIntermediate, TNew> projector)
        => (await task).SelectMany(binder, projector);

    public static async Task<AxisResult<TNew>> SelectManyAsync<TValue, TIntermediate, TNew>(
        this Task<AxisResult<TValue>> task,
        Func<TValue, Task<AxisResult<TIntermediate>>> binder,
        Func<TValue, TIntermediate, TNew> projector)
        => await (await task).SelectManyAsync(binder, projector);

    public static async Task<AxisResult> RequireNotFoundAsync<TValue>(this Task<AxisResult<TValue>> task, AxisError errorIfFound)
        => (await task).RequireNotFound(errorIfFound);

    // --- Task<AxisResult<(T1, T2)>> Tuple2 extensions ---

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2));

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, Task<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2));

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, T3> mapper)
    {
        var r = await task;
        return r.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, mapper(r.Value.Value1, r.Value.Value2)))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, Task<T3>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var v3 = await mapper(r.Value.Value1, r.Value.Value2);
        return AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, v3));
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, AxisResult<T3>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var m = mapper(r.Value.Value1, r.Value.Value2);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T1, T2, T3>(this Task<AxisResult<(T1 Value1, T2 Value2)>> task, Func<T1, T2, Task<AxisResult<T3>>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        var m = await mapper(r.Value.Value1, r.Value.Value2);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
    }

    // --- Task<AxisResult<(T1, T2, T3)>> Tuple3 extensions ---

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, T3, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, T3, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, Task<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, T4> mapper)
    {
        var r = await task;
        return r.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3)))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, Task<T4>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var v4 = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, v4));
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, AxisResult<T4>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var m = mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
    }

    public static async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T1, T2, T3, T4>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task, Func<T1, T2, T3, Task<AxisResult<T4>>> mapper)
    {
        var r = await task;
        if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        var m = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
        return m.IsSuccess
            ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
            : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
    }

    // --- Task<AxisResult<(T1, T2, T3, T4)>> Tuple4 extensions ---

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, T3, T4, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task, Func<T1, T2, T3, T4, TNew> mapper)
        => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));

    public static async Task<AxisResult<TNew>> MapAsync<T1, T2, T3, T4, TNew>(this Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task, Func<T1, T2, T3, T4, Task<TNew>> mapper)
        => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));

    #endregion
}
