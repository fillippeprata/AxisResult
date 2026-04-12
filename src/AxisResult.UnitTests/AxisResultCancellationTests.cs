namespace AxisResult.UnitTests;

public class AxisResultCancellationTests
{
    private static readonly AxisError E1 = AxisError.NotFound("NF");

    #region CancellationToken flows through the pipeline (Task)

    [Fact]
    public async Task ThenAsync_Task_Forwards_CancellationToken_To_Delegate()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(1).AsTaskAsync()
            .ThenAsync((v, ct) =>
            {
                observed = ct;
                return Task.FromResult(AxisResult.Ok(v + 1));
            }, cts.Token);

        Assert.Equal(2, result.Value);
        Assert.Equal(cts.Token, observed);
    }

    [Fact]
    public async Task MapAsync_Task_Forwards_CancellationToken_To_Delegate()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(5).AsTaskAsync()
            .MapAsync((v, ct) =>
            {
                observed = ct;
                return Task.FromResult(v * 2);
            }, cts.Token);

        Assert.Equal(10, result.Value);
        Assert.Equal(cts.Token, observed);
    }

    [Fact]
    public async Task EnsureAsync_Predicate_Task_Forwards_CancellationToken()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(10).AsTaskAsync()
            .EnsureAsync((v, ct) =>
            {
                observed = ct;
                return Task.FromResult(v > 0);
            }, E1, cts.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal(cts.Token, observed);
    }

    [Fact]
    public async Task Cancelled_Token_Before_Step_Triggers_OperationCanceled_In_Delegate()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await AxisResult.Ok(1).AsTaskAsync()
                .ThenAsync((v, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    return Task.FromResult(AxisResult.Ok(v + 1));
                }, cts.Token));
    }

    [Fact]
    public async Task ThenAsync_Task_Failure_Does_Not_Invoke_Delegate_Even_With_Token()
    {
        var invoked = false;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Error<int>(E1).AsTaskAsync()
            .ThenAsync((v, ct) =>
            {
                invoked = true;
                return Task.FromResult(AxisResult.Ok(v + 1));
            }, cts.Token);

        Assert.True(result.IsFailure);
        Assert.False(invoked);
    }

    [Fact]
    public async Task ActionAsync_Task_Preserves_Value_And_Forwards_Token()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok("kept").AsTaskAsync()
            .ActionAsync((v, ct) =>
            {
                observed = ct;
                return Task.FromResult(AxisResult.Ok());
            }, cts.Token);

        Assert.Equal("kept", result.Value);
        Assert.Equal(cts.Token, observed);
    }

    [Fact]
    public async Task ZipAsync_Task_Forwards_Token_To_Failable_Mapper()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(1).AsTaskAsync()
            .ZipAsync((v, ct) =>
            {
                observed = ct;
                return Task.FromResult(AxisResult.Ok(v + 1));
            }, cts.Token);

        Assert.True(result.IsSuccess);
        Assert.Equal((1, 2), result.Value);
        Assert.Equal(cts.Token, observed);
    }

    #endregion

    #region CancellationToken flows through the pipeline (ValueTask)

    [Fact]
    public async Task ThenAsync_ValueTask_Forwards_CancellationToken_To_Delegate()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(1).AsValueTaskAsync()
            .ThenAsync((v, ct) =>
            {
                observed = ct;
                return new ValueTask<AxisResult<int>>(AxisResult.Ok(v + 1));
            }, cts.Token);

        Assert.Equal(2, result.Value);
        Assert.Equal(cts.Token, observed);
    }

    [Fact]
    public async Task MapAsync_ValueTask_Forwards_CancellationToken_To_Delegate()
    {
        CancellationToken observed = default;
        using var cts = new CancellationTokenSource();

        var result = await AxisResult.Ok(3).AsValueTaskAsync()
            .MapAsync((v, ct) =>
            {
                observed = ct;
                return new ValueTask<int>(v + 10);
            }, cts.Token);

        Assert.Equal(13, result.Value);
        Assert.Equal(cts.Token, observed);
    }

    #endregion
}
