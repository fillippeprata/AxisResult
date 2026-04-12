namespace AxisTrix;

public abstract partial class AxisResult
{
    #region Sync

    public static AxisResult Ok() => new AxisResultImpl();
    public static AxisResult<TValue> Ok<TValue>(TValue value) => new AxisResultImpl<TValue>(value);

    public static AxisResult Error(AxisError error) => new AxisResultImpl([error]);
    public static AxisResult Error(IEnumerable<AxisError> errors) => new AxisResultImpl(errors.ToList());

    public static AxisResult<TValue> Error<TValue>(AxisError error) => new AxisResultImpl<TValue>(default, [error]);
    public static AxisResult<TValue> Error<TValue>(IEnumerable<AxisError> errors) => new AxisResultImpl<TValue>(default, errors.ToList());

    public static AxisResult Combine(params AxisResult[] results)
    {
        var errors = results.Where(r => r.IsFailure).SelectMany(r => r.Errors).ToList();
        return errors.Count == 0 ? Ok() : Error(errors);
    }

    public static AxisResult Combine(IEnumerable<AxisResult> results)
    {
        var errors = results.Where(r => r.IsFailure).SelectMany(r => r.Errors).ToList();
        return errors.Count == 0 ? Ok() : Error(errors);
    }

    public static AxisResult<IReadOnlyList<TValue>> All<TValue>(IEnumerable<AxisResult<TValue>> results)
    {
        var resultList = results.ToList();
        var errors = resultList.Where(r => r.IsFailure).SelectMany(r => r.Errors).ToList();
        return errors.Count != 0
            ? Error<IReadOnlyList<TValue>>(errors)
            : Ok<IReadOnlyList<TValue>>(resultList.Select(r => r.Value).ToList());
    }

    internal static AxisResult<TNew> PropagateErrors<TNew>(AxisResult source) => new AxisResultImpl<TNew>(default, source.RawErrors);

    #endregion

    #region Async

    public static Task<AxisResult> OkAsync() => Task.FromResult(Ok());
    public static Task<AxisResult<TValue>> OkAsync<TValue>(TValue value) => Task.FromResult(Ok(value));

    public static async Task<AxisResult<IReadOnlyList<TValue>>> AllAsync<TValue>(IEnumerable<Task<AxisResult<TValue>>> tasks)
    {
        var results = await Task.WhenAll(tasks);
        return All(results);
    }

    public static async Task<AxisResult> CombineAsync(IEnumerable<Task<AxisResult>> tasks)
    {
        var results = await Task.WhenAll(tasks);
        return Combine(results);
    }

    public static async ValueTask<AxisResult<IReadOnlyList<TValue>>> AllAsync<TValue>(IEnumerable<ValueTask<AxisResult<TValue>>> tasks)
    {
        var list = new List<AxisResult<TValue>>();
        foreach (var t in tasks) list.Add(await t);
        return All(list);
    }

    public static async ValueTask<AxisResult> CombineAsync(IEnumerable<ValueTask<AxisResult>> tasks)
    {
        var list = new List<AxisResult>();
        foreach (var t in tasks) list.Add(await t);
        return Combine(list);
    }

    #endregion

    #region Try

    private static bool IsCritical(Exception ex) => ex is
        OperationCanceledException or
        StackOverflowException or
        OutOfMemoryException or
        ThreadAbortException or
        NullReferenceException or
        ArgumentNullException;

    public static AxisResult Try(Action action, Func<Exception, AxisError>? errorHandler = null)
    {
        try { action(); return Ok(); }
        catch (Exception ex) when (!IsCritical(ex)) { return Error(errorHandler?.Invoke(ex) ?? AxisError.InternalServerError(ex.Message)); }
    }
    public static async Task<AxisResult> TryAsync(Func<Task> action, Func<Exception, AxisError>? errorHandler = null)
    {
        try { await action(); return await OkAsync(); }
        catch (Exception ex) when (!IsCritical(ex)) { return errorHandler?.Invoke(ex) ?? AxisError.InternalServerError(ex.Message); }
    }

    public static AxisResult<TValue> Try<TValue>(Func<TValue> func, Func<Exception, AxisError>? errorHandler = null)
    {
        try { return Ok(func()); }
        catch (Exception ex) when (!IsCritical(ex)) { return Error<TValue>(errorHandler?.Invoke(ex) ?? AxisError.InternalServerError(ex.Message)); }
    }
    public static async Task<AxisResult<TValue>> TryAsync<TValue>(Func<Task<TValue>> func, Func<Exception, AxisError>? errorHandler = null)
    {
        try { return Ok(await func()); }
        catch (Exception ex) when (!IsCritical(ex)) { return Error<TValue>(errorHandler?.Invoke(ex) ?? AxisError.InternalServerError(ex.Message)); }
    }

    #endregion
}
