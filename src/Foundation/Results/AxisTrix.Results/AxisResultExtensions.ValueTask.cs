namespace AxisTrix;

public static class AxisResultValueTaskExtensions
{
    extension(AxisResult axisResult)
    {
        public ValueTask<AxisResult> AsValueTaskAsync() => new(axisResult);
    }

    extension<TValue>(AxisResult<TValue> axisResult)
    {
        public ValueTask<AxisResult<TValue>> AsValueTaskAsync() => new(axisResult);
    }

    #region ValueTask Extensions (Railway Oriented)

    extension(ValueTask<AxisResult> task)
    {
        public async ValueTask<AxisResult> ThenAsync(Func<AxisResult> next)
            => (await task).Then(next);
        public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<AxisResult<TNew>> next)
            => (await task).Then(next);
        public async ValueTask<AxisResult> ThenAsync(Func<ValueTask<AxisResult>> next)
            => await (await task).ThenAsync(next);
        public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<ValueTask<AxisResult<TNew>>> next)
            => await (await task).ThenAsync(next);

        public async ValueTask<AxisResult> TapAsync(Action action)
            => (await task).Tap(action);
        public async ValueTask<AxisResult> TapAsync(Func<ValueTask> action)
            => await (await task).TapAsync(action);

        public async ValueTask<AxisResult> TapErrorAsync(Action<IReadOnlyList<AxisError>> action)
            => (await task).TapError(action);
        public async ValueTask<AxisResult> TapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask> action)
            => await (await task).TapErrorAsync(action);

        public async ValueTask<TResult> MatchAsync<TResult>(Func<TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
            => (await task).Match(onSuccess, onFailure);
        public async ValueTask<TResult> MatchAsync<TResult>(Func<ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
            => await (await task).MatchAsync(onSuccess, onFailure);

        public async ValueTask<AxisResult> RequireNotFoundAsync(AxisError errorIfFound)
            => (await task).RequireNotFound(errorIfFound);
    }

    extension<TValue>(ValueTask<AxisResult<TValue>> task)
    {
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, TNew> mapper)
            => (await task).Map(mapper);
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<TValue, ValueTask<TNew>> mapper)
            => await (await task).MapAsync(mapper);

        public async ValueTask<AxisResult> ThenAsync(Func<TValue, AxisResult> next)
            => (await task).Then(next);
        public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<TValue, AxisResult<TNew>> next)
            => (await task).Then(next);
        public async ValueTask<AxisResult> ThenAsync(Func<TValue, ValueTask<AxisResult>> next)
            => await (await task).ThenAsync(next);
        public async ValueTask<AxisResult<TNew>> ThenAsync<TNew>(Func<TValue, ValueTask<AxisResult<TNew>>> next)
            => await (await task).ThenAsync(next);

        public async ValueTask<AxisResult<TValue>> TapAsync(Action<TValue> action)
            => (await task).Tap(action);
        public async ValueTask<AxisResult<TValue>> TapAsync(Func<TValue, ValueTask> action)
            => await (await task).TapAsync(action);

        public async ValueTask<AxisResult<TValue>> TapErrorAsync(Action<IReadOnlyList<AxisError>> action)
            => (await task).TapError(action);
        public async ValueTask<AxisResult<TValue>> TapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask> action)
            => await (await task).TapErrorAsync(action);

        public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, bool> predicate, AxisError error)
            => (await task).Ensure(predicate, error);
        public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, ValueTask<bool>> predicate, AxisError error)
            => await (await task).EnsureAsync(predicate, error);
        public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, AxisResult> validation)
            => (await task).Ensure(validation);
        public async ValueTask<AxisResult<TValue>> EnsureAsync(Func<TValue, ValueTask<AxisResult>> validation)
            => await (await task).EnsureAsync(validation);

        public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, TNew> mapper)
            => (await task).Zip(mapper);
        public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, ValueTask<TNew>> mapper)
            => await (await task).ZipAsync(mapper);
        public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, AxisResult<TNew>> mapper)
            => (await task).Zip(mapper);
        public async ValueTask<AxisResult<(TValue Value1, TNew Value2)>> ZipAsync<TNew>(Func<TValue, ValueTask<AxisResult<TNew>>> mapper)
            => await (await task).ZipAsync(mapper);

        public async ValueTask<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, IEnumerable<AxisError>> mapper)
            => (await task).MapError(mapper);
        public async ValueTask<AxisResult<TValue>> MapErrorAsync(Func<AxisError, AxisError> mapper)
            => (await task).MapError(mapper);
        public async ValueTask<AxisResult<TValue>> MapErrorAsync(Func<IReadOnlyList<AxisError>, ValueTask<IEnumerable<AxisError>>> mapper)
            => await (await task).MapErrorAsync(mapper);

        public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, TValue> recovery)
            => (await task).Recover(recovery);
        public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
            => await (await task).RecoverAsync(recovery);
        public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<TValue> recovery)
            => (await task).Recover(recovery);
        public async ValueTask<AxisResult<TValue>> RecoverAsync(Func<ValueTask<TValue>> recovery)
            => await (await task).RecoverAsync(recovery);
        public async ValueTask<AxisResult<TValue>> RecoverAsync(TValue defaultValue)
            => (await task).Recover(defaultValue);

        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, TValue> recovery)
            => (await task).RecoverWhen(predicate, recovery);
        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(Func<IReadOnlyList<AxisError>, bool> predicate, Func<IReadOnlyList<AxisError>, ValueTask<TValue>> recovery)
            => await (await task).RecoverWhenAsync(predicate, recovery);
        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<TValue> recovery)
            => (await task).RecoverWhen(type, recovery);
        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(AxisErrorType type, Func<ValueTask<TValue>> recovery)
            => await (await task).RecoverWhenAsync(type, recovery);
        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(string code, Func<TValue> recovery)
            => (await task).RecoverWhen(code, recovery);
        public async ValueTask<AxisResult<TValue>> RecoverWhenAsync(string code, Func<ValueTask<TValue>> recovery)
            => await (await task).RecoverWhenAsync(code, recovery);

        public async ValueTask<AxisResult<TValue>> RecoverNotFoundAsync(Func<TValue> recovery)
            => (await task).RecoverNotFound(recovery);
        public async ValueTask<AxisResult<TValue>> RecoverNotFoundAsync(Func<ValueTask<TValue>> recovery)
            => await (await task).RecoverNotFoundAsync(recovery);

        public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback)
            => (await task).OrElse(fallback);
        public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback)
            => await (await task).OrElseAsync(fallback);
        public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, AxisResult<TValue>> fallback, bool combineErrors)
            => (await task).OrElse(fallback, combineErrors);
        public async ValueTask<AxisResult<TValue>> OrElseAsync(Func<IReadOnlyList<AxisError>, ValueTask<AxisResult<TValue>>> fallback, bool combineErrors)
            => await (await task).OrElseAsync(fallback, combineErrors);

        public async ValueTask<TResult> MatchAsync<TResult>(Func<TValue, TResult> onSuccess, Func<IReadOnlyList<AxisError>, TResult> onFailure)
            => (await task).Match(onSuccess, onFailure);
        public async ValueTask<TResult> MatchAsync<TResult>(Func<TValue, ValueTask<TResult>> onSuccess, Func<IReadOnlyList<AxisError>, ValueTask<TResult>> onFailure)
            => await (await task).MatchAsync(onSuccess, onFailure);

        public async ValueTask<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
            Func<TValue, AxisResult<TIntermediate>> binder,
            Func<TValue, TIntermediate, TNew> projector)
            => (await task).SelectMany(binder, projector);
        public async ValueTask<AxisResult<TNew>> SelectManyAsync<TIntermediate, TNew>(
            Func<TValue, ValueTask<AxisResult<TIntermediate>>> binder,
            Func<TValue, TIntermediate, TNew> projector)
            => await (await task).SelectManyAsync(binder, projector);

        public async ValueTask<AxisResult> RequireNotFoundAsync(AxisError errorIfFound)
            => (await task).RequireNotFound(errorIfFound);
    }

    extension<T1, T2>(ValueTask<AxisResult<(T1 Value1, T2 Value2)>> task)
    {
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2));
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, ValueTask<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2));

        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, T3> mapper)
        {
            var r = await task;
            return r.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, mapper(r.Value.Value1, r.Value.Value2)))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, ValueTask<T3>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var v3 = await mapper(r.Value.Value1, r.Value.Value2);
            return AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, v3));
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, AxisResult<T3>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var m = mapper(r.Value.Value1, r.Value.Value2);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> ZipAsync<T3>(Func<T1, T2, ValueTask<AxisResult<T3>>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(r.Errors);
            var m = await mapper(r.Value.Value1, r.Value.Value2);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3)>((r.Value.Value1, r.Value.Value2, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3)>(m.Errors);
        }
    }

    extension<T1, T2, T3>(ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3)>> task)
    {
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, ValueTask<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3));

        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, T4> mapper)
        {
            var r = await task;
            return r.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3)))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, ValueTask<T4>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var v4 = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, v4));
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, AxisResult<T4>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var m = mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
        }
        public async ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> ZipAsync<T4>(Func<T1, T2, T3, ValueTask<AxisResult<T4>>> mapper)
        {
            var r = await task;
            if (r.IsFailure) return AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(r.Errors);
            var m = await mapper(r.Value.Value1, r.Value.Value2, r.Value.Value3);
            return m.IsSuccess
                ? AxisResult.Ok<(T1, T2, T3, T4)>((r.Value.Value1, r.Value.Value2, r.Value.Value3, m.Value))
                : AxisResult.Error<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>(m.Errors);
        }
    }

    extension<T1, T2, T3, T4>(ValueTask<AxisResult<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>> task)
    {
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, T4, TNew> mapper)
            => (await task).Map(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));
        public async ValueTask<AxisResult<TNew>> MapAsync<TNew>(Func<T1, T2, T3, T4, ValueTask<TNew>> mapper)
            => await (await task).MapAsync(tuple => mapper(tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4));
    }

    #endregion
}
