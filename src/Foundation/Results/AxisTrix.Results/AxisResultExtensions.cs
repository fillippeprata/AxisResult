namespace AxisResult;

public static class AxisResultExtensions
{
    extension(AxisResult axisResult)
    {
        public Task<AxisResult> AsTaskAsync() => Task.FromResult(axisResult);
    }

    extension<TValue>(AxisResult<TValue> axisResult)
    {
        public Task<AxisResult<TValue>> AsTaskAsync() => Task.FromResult(axisResult);
    }

    #region Task Extensions (Railway Oriented)

    extension(Task<AxisResult> task)
    {
        public async Task<AxisResult> ThenAsync(Func<AxisResult> next)
            => (await task).Then(next);

        public async Task<AxisResult<TNew>> WithValueAsync<TNew>(TNew value)
        {
            var result = await task;
            return result.IsSuccess ? AxisResult.Ok(value) : result.Errors.ToArray();
        }

        public async Task<AxisResult<TNew>> ThenAsync<TNew>(Func<AxisResult<TNew>> next)
            => (await task).Then(next);

        public async Task<AxisResult> ThenAsync(Func<Task<AxisResult>> next)
            => await (await task).ThenAsync(next);

        public async Task<AxisResult<TNew>> ThenAsync<TNew>(Func<Task<AxisResult<TNew>>> next)
            => await (await task).ThenAsync(next);

        public async Task<AxisResult> TapAsync(Action action)
            => (await task).Tap(action);

        public async Task<AxisResult> TapAsync(Func<Task> action)
            => await (await task).TapAsync(action);

        public async Task<AxisResult> TapErrorAsync(Action<IReadOnlyList<AxisError>> action)
            => (await task).TapError(action);

        public async Task<AxisResult> TapErrorAsync(Func<IReadOnlyList<AxisError>, Task> action)
            => await (await task).TapErrorAsync(action);

        public async Task<TResult> MatchAsync<TResult>(Func<TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
            => (await task).Match(onSuccess, onFailure);

        public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure)
            => await (await task).MatchAsync(onSuccess, onFailure);

        public async Task<AxisResult> RequireNotFoundAsync(AxisError errorIfFound)
            => (await task).RequireNotFound(errorIfFound);
    }

    extension<TValue>(Task<AxisResult<TValue>> task)
    {
        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, TNew> mapper)
            => (await task).Map(mapper);

        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, Task<TNew>> mapper)
            => await (await task).MapAsync(mapper);

        public async Task<AxisResult<TValue>> ActionAsync(Func<TValue, Task<AxisResult>> next)
            => await (await task).ThenAsync(next);

        public async Task<AxisResult<TValue>> ThenAsync(Func<TValue, AxisResult> next)
            => (await task).Then(next);

        public async Task<AxisResult<TValue>> ThenAsync(Func<TValue, Task<AxisResult>> next)
            => await (await task).ThenAsync(next);

        public async Task<AxisResult<TValue>> TapAsync(Action<TValue> action)
            => (await task).Tap(action);

        public async Task<AxisResult<TValue>> TapAsync(Func<TValue, Task> action)
            => await (await task).TapAsync(action);

        public async Task<AxisResult<TValue>> TapErrorAsync(Action<IReadOnlyList<AxisError>> action)
            => (await task).TapError(action);

        public async Task<AxisResult<TValue>> TapErrorAsync(Func<IReadOnlyList<AxisError>, Task> action)
            => await (await task).TapErrorAsync(action);

        public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, bool> predicate, AxisError error)
            => (await task).Ensure(predicate, error);

        public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, Task<bool>> predicate, AxisError error)
            => await (await task).EnsureAsync(predicate, error);

        public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, AxisResult> validation)
            => (await task).Ensure(validation);

        public async Task<AxisResult<TValue>> EnsureAsync(Func<TValue, Task<AxisResult>> validation)
            => await (await task).EnsureAsync(validation);

        public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, TNew> mapper)
            => (await task).Zip(mapper);

        public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, Task<TNew>> mapper)
            => await (await task).ZipAsync(mapper);

        public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, AxisResult<TNew>> mapper)
            => (await task).Zip(mapper);

        public async Task<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, Task<AxisResult<TNew>>> mapper)
            => await (await task).ZipAsync(mapper);

        public async Task<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
            => (await task).MapError(mapper);

        public async Task<AxisResult<TValue>> MapErrorAsync(Func<AxisError, AxisError> mapper)
            => (await task).MapError(mapper);

        public async Task<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, Task<IEnumerable<AxisError>>> mapper)
            => await (await task).MapErrorAsync(mapper);

        public async Task<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, TValue> recovery)
            => (await task).Recover(recovery);

        public async Task<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
            => await (await task).RecoverAsync(recovery);

        public async Task<AxisResult<TValue>> RecoverAsync(Func<TValue> recovery)
            => (await task).Recover(recovery);

        public async Task<AxisResult<TValue>> RecoverAsync(Func<Task<TValue>> recovery)
            => await (await task).RecoverAsync(recovery);

        public async Task<AxisResult<TValue>> RecoverAsync(TValue defaultValue)
            => (await task).Recover(defaultValue);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, TValue> recovery)
            => (await task).RecoverWhen(predicate, recovery);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, Task<TValue>> recovery)
            => await (await task).RecoverWhenAsync(predicate, recovery);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<TValue> recovery)
            => (await task).RecoverWhen(type, recovery);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<Task<TValue>> recovery)
            => await (await task).RecoverWhenAsync(type, recovery);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(string code, Func<TValue> recovery)
            => (await task).RecoverWhen(code, recovery);

        public async Task<AxisResult<TValue>> RecoverWhenAsync(string code, Func<Task<TValue>> recovery)
            => await (await task).RecoverWhenAsync(code, recovery);

        public async Task<AxisResult<TValue>> RecoverNotFoundAsync(Func<TValue> recovery)
            => (await task).RecoverNotFound(recovery);

        public async Task<AxisResult<TValue>> RecoverNotFoundAsync(Func<Task<TValue>> recovery)
            => await (await task).RecoverNotFoundAsync(recovery);

        public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback)
            => (await task).OrElse(fallback);

        public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback)
            => await (await task).OrElseAsync(fallback);

        public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback, bool combineErrors)
            => (await task).OrElse(fallback, combineErrors);

        public async Task<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, Task<AxisResult<TValue>>> fallback, bool combineErrors)
            => await (await task).OrElseAsync(fallback, combineErrors);

        public async Task<TResult> MatchAsync<TResult>(Func<TValue, TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
            => (await task).Match(onSuccess, onFailure);

        public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, Task<TResult>> onFailure)
            => await (await task).MatchAsync(onSuccess, onFailure);

        public async Task<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
            Func<TValue, AxisResult<TIntermediate>> binder,
            Func<TValue, TIntermediate, TNew> projector)
            => (await task).SelectMany(binder, projector);

        public async Task<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
            Func<TValue, Task<AxisResult<TIntermediate>>> binder,
            Func<TValue, TIntermediate, TNew> projector)
            => await (await task).SelectManyAsync(binder, projector);

        public async Task<AxisResult> RequireNotFoundAsync(AxisError errorIfFound)
            => (await task).RequireNotFound(errorIfFound);
    }

    extension<T1, T2>(Task<AxisResult<(T1 Value1, T2 Value2)>> task)
    {
        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2));

        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, Task<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2));

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, T3> mapper)
        {
            var r = await task;
            return r.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, mapper(r.Value.Value1, r.Value.Value2)))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, Task<T3>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var v3 = await mapper(r.Value.Value1, r.Value.Value2);
            return AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, v3));
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, AxisResult<T3>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var m = mapper(r.Value.Value1, r.Value.Value2);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, Task<AxisResult<T3>>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var m = await mapper(r.Value.Value1, r.Value.Value2);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
        }
    }

    extension<T1, T2, T3>(Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task)
    {
        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, Task<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, T4> mapper)
        {
            var r = await task;
            return r.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3)))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, Task<T4>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var v4 = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, v4));
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, AxisResult<T4>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var m = mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
        }

        public async Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, Task<AxisResult<T4>>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var m = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
        }
    }

    extension<T1, T2, T3, T4>(Task<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task)
    {
        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, T4, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));

        public async Task<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, T4, Task<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));
    }

    #endregion
}
