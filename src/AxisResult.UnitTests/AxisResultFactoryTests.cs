using AxisResult;

namespace AxisTrix.Results.UnitTests;

public class AxisResultFactoryTests
{
    private static readonly AxisError E1 = AxisError.NotFound("E1");
    private static readonly AxisError E2 = AxisError.ValidationRule("E2");

    #region Combine

    [Fact]
    public void Combine_Params_All_Success_Returns_Ok()
    {
        var r = AxisResult.AxisResult.Combine(AxisResult.AxisResult.Ok(), AxisResult.AxisResult.Ok(), AxisResult.AxisResult.Ok());
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public void Combine_Params_Mixed_Returns_Failure_With_All_Errors()
    {
        var r = AxisResult.AxisResult.Combine(
            AxisResult.AxisResult.Ok(),
            AxisResult.AxisResult.Error(E1),
            AxisResult.AxisResult.Error(E2));
        Assert.True(r.IsFailure);
        Assert.Equal(2, r.Errors.Count);
    }

    [Fact]
    public void Combine_Params_Empty_Returns_Ok()
    {
        var r = AxisResult.AxisResult.Combine();
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public void Combine_Enumerable_All_Success_Returns_Ok()
    {
        IEnumerable<AxisResult.AxisResult> list = new[] { AxisResult.AxisResult.Ok(), AxisResult.AxisResult.Ok() };
        var r = AxisResult.AxisResult.Combine(list);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public void Combine_Enumerable_With_Failures_Returns_Aggregate()
    {
        IEnumerable<AxisResult.AxisResult> list = new[]
        {
            AxisResult.AxisResult.Error(E1),
            AxisResult.AxisResult.Ok(),
            AxisResult.AxisResult.Error(E2)
        };
        var r = AxisResult.AxisResult.Combine(list);
        Assert.Equal(2, r.Errors.Count);
    }

    #endregion

    #region All

    [Fact]
    public void All_All_Success_Returns_Values()
    {
        var results = new[] { AxisResult.AxisResult.Ok(1), AxisResult.AxisResult.Ok(2), AxisResult.AxisResult.Ok(3) };
        var r = AxisResult.AxisResult.All(results);
        Assert.True(r.IsSuccess);
        Assert.Equal(new[] { 1, 2, 3 }, r.Value);
    }

    [Fact]
    public void All_With_Failure_Returns_Aggregate_Errors()
    {
        var results = new[]
        {
            AxisResult.AxisResult.Ok(1),
            AxisResult.AxisResult.Error<int>(E1),
            AxisResult.AxisResult.Error<int>(E2)
        };
        var r = AxisResult.AxisResult.All(results);
        Assert.True(r.IsFailure);
        Assert.Equal(2, r.Errors.Count);
    }

    [Fact]
    public void All_Returns_IReadOnlyList()
    {
        var r = AxisResult.AxisResult.All(new[] { AxisResult.AxisResult.Ok(1), AxisResult.AxisResult.Ok(2) });
        Assert.IsAssignableFrom<IReadOnlyList<int>>(r.Value);
        Assert.Equal(2, r.Value.Count);
    }

    [Fact]
    public void All_Empty_Returns_Empty_Ok()
    {
        var r = AxisResult.AxisResult.All(Array.Empty<AxisResult<int>>());
        Assert.True(r.IsSuccess);
        Assert.Empty(r.Value);
    }

    #endregion

    #region AllAsync / CombineAsync Task

    [Fact]
    public async Task AllAsync_Task_All_Success()
    {
        var tasks = new[]
        {
            Task.FromResult(AxisResult.AxisResult.Ok(1)),
            Task.FromResult(AxisResult.AxisResult.Ok(2))
        };
        var r = await AxisResult.AxisResult.AllAsync(tasks);
        Assert.True(r.IsSuccess);
        Assert.Equal(new[] { 1, 2 }, r.Value);
    }

    [Fact]
    public async Task AllAsync_Task_With_Failures()
    {
        var tasks = new[]
        {
            Task.FromResult(AxisResult.AxisResult.Ok(1)),
            Task.FromResult(AxisResult.AxisResult.Error<int>(E1))
        };
        var r = await AxisResult.AxisResult.AllAsync(tasks);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task CombineAsync_Task_All_Success()
    {
        var tasks = new[]
        {
            Task.FromResult(AxisResult.AxisResult.Ok()),
            Task.FromResult(AxisResult.AxisResult.Ok())
        };
        var r = await AxisResult.AxisResult.CombineAsync(tasks);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task CombineAsync_Task_With_Failures()
    {
        var tasks = new[]
        {
            Task.FromResult(AxisResult.AxisResult.Ok()),
            Task.FromResult(AxisResult.AxisResult.Error(E1))
        };
        var r = await AxisResult.AxisResult.CombineAsync(tasks);
        Assert.True(r.IsFailure);
    }

    #endregion

    #region AllAsync / CombineAsync ValueTask

    [Fact]
    public async Task AllAsync_ValueTask_All_Success()
    {
        var tasks = new[]
        {
            new ValueTask<AxisResult<int>>(AxisResult.AxisResult.Ok(1)),
            new ValueTask<AxisResult<int>>(AxisResult.AxisResult.Ok(2))
        };
        var r = await AxisResult.AxisResult.AllAsync(tasks);
        Assert.True(r.IsSuccess);
        Assert.Equal(2, r.Value.Count);
    }

    [Fact]
    public async Task AllAsync_ValueTask_With_Failures()
    {
        var tasks = new[]
        {
            new ValueTask<AxisResult<int>>(AxisResult.AxisResult.Ok(1)),
            new ValueTask<AxisResult<int>>(AxisResult.AxisResult.Error<int>(E1))
        };
        var r = await AxisResult.AxisResult.AllAsync(tasks);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task CombineAsync_ValueTask_All_Success()
    {
        var tasks = new[]
        {
            new ValueTask<AxisResult.AxisResult>(AxisResult.AxisResult.Ok()),
            new ValueTask<AxisResult.AxisResult>(AxisResult.AxisResult.Ok())
        };
        var r = await AxisResult.AxisResult.CombineAsync(tasks);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task CombineAsync_ValueTask_With_Failures()
    {
        var tasks = new[]
        {
            new ValueTask<AxisResult.AxisResult>(AxisResult.AxisResult.Error(E1)),
            new ValueTask<AxisResult.AxisResult>(AxisResult.AxisResult.Ok())
        };
        var r = await AxisResult.AxisResult.CombineAsync(tasks);
        Assert.True(r.IsFailure);
    }

    #endregion

    #region Try

    [Fact]
    public void Try_Action_Success()
    {
        var called = false;
        var r = AxisResult.AxisResult.Try(() => called = true);
        Assert.True(r.IsSuccess);
        Assert.True(called);
    }

    [Fact]
    public void Try_Action_Captures_Exception()
    {
        var r = AxisResult.AxisResult.Try(() => throw new InvalidOperationException("boom"));
        Assert.True(r.IsFailure);
        Assert.Contains("boom", r.Errors[0].Code);
        Assert.Equal(AxisErrorType.InternalServerError, r.Errors[0].Type);
    }

    [Fact]
    public void Try_Action_Uses_Custom_Handler()
    {
        var r = AxisResult.AxisResult.Try(
            () => throw new InvalidOperationException("x"),
            _ => AxisError.ValidationRule("CUSTOM"));
        Assert.True(r.IsFailure);
        Assert.Equal("CUSTOM", r.Errors[0].Code);
        Assert.Equal(AxisErrorType.ValidationRule, r.Errors[0].Type);
    }

    [Fact]
    public void Try_Func_Success_Returns_Value()
    {
        var r = AxisResult.AxisResult.Try(() => 42);
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public void Try_Func_Captures_Exception()
    {
        var r = AxisResult.AxisResult.Try<int>(() => throw new InvalidOperationException("boom"));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public void Try_Func_Uses_Custom_Handler()
    {
        var r = AxisResult.AxisResult.Try<int>(
            () => throw new InvalidOperationException("x"),
            _ => AxisError.BusinessRule("B1"));
        Assert.Equal("B1", r.Errors[0].Code);
    }

    [Fact]
    public void Try_Rethrows_OperationCanceledException()
    {
        Assert.Throws<OperationCanceledException>(
            () => AxisResult.AxisResult.Try(() => throw new OperationCanceledException()));
    }

    [Fact]
    public void Try_Rethrows_NullReferenceException()
    {
        Assert.Throws<NullReferenceException>(
            () => AxisResult.AxisResult.Try(() => throw new NullReferenceException()));
    }

    [Fact]
    public void Try_Rethrows_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => AxisResult.AxisResult.Try(() => throw new ArgumentNullException()));
    }

    [Fact]
    public void Try_Generic_Rethrows_OperationCanceledException()
    {
        Assert.Throws<OperationCanceledException>(
            () => AxisResult.AxisResult.Try<int>(() => throw new OperationCanceledException()));
    }

    [Fact]
    public async Task TryAsync_Action_Success()
    {
        var r = await AxisResult.AxisResult.TryAsync(() => Task.CompletedTask);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task TryAsync_Action_Captures_Exception()
    {
        var r = await AxisResult.AxisResult.TryAsync(() => Task.FromException(new InvalidOperationException("e")));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task TryAsync_Action_Uses_Custom_Handler()
    {
        var r = await AxisResult.AxisResult.TryAsync(
            () => Task.FromException(new InvalidOperationException("e")),
            _ => AxisError.Timeout("T"));
        Assert.Equal("T", r.Errors[0].Code);
        Assert.Equal(AxisErrorType.Timeout, r.Errors[0].Type);
    }

    [Fact]
    public async Task TryAsync_Func_Success_Returns_Value()
    {
        var r = await AxisResult.AxisResult.TryAsync(() => Task.FromResult(10));
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task TryAsync_Func_Captures_Exception()
    {
        var r = await AxisResult.AxisResult.TryAsync(() => Task.FromException<int>(new InvalidOperationException("e")));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task TryAsync_Func_Uses_Custom_Handler()
    {
        var r = await AxisResult.AxisResult.TryAsync(
            () => Task.FromException<int>(new InvalidOperationException("e")),
            _ => AxisError.Conflict("C"));
        Assert.Equal("C", r.Errors[0].Code);
    }

    [Fact]
    public async Task TryAsync_Rethrows_Cancellation()
    {
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => AxisResult.AxisResult.TryAsync(() => Task.FromException(new OperationCanceledException())));
    }

    [Fact]
    public async Task TryAsync_Func_Rethrows_Cancellation()
    {
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => AxisResult.AxisResult.TryAsync(() => Task.FromException<int>(new TaskCanceledException())));
    }

    #endregion
}
