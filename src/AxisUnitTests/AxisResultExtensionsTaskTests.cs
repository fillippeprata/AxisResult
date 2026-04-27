using Axis;

namespace AxisUnitTests;

public class AxisResultExtensionsTaskTests
{
    private static readonly AxisError E1 = AxisError.NotFound("E1");
    private static readonly AxisError E2 = AxisError.ValidationRule("E2");

    private static Task<AxisResult> TOkAsync() => Task.FromResult(AxisResult.Ok());
    private static Task<AxisResult<T>> TOkAsync<T>(T v) => Task.FromResult(AxisResult.Ok(v));
    private static Task<AxisResult> TErrAsync(AxisError e) => Task.FromResult(AxisResult.Error(e));
    private static Task<AxisResult<T>> TErrAsync<T>(AxisError e) => Task.FromResult(AxisResult.Error<T>(e));

    #region AsTask

    [Fact]
    public async Task AsTask_NonGeneric()
    {
        var t = AxisResult.Ok().AsTaskAsync();
        var r = await t;
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task AsTask_Generic()
    {
        var t = AxisResult.Ok(5).AsTaskAsync();
        var r = await t;
        Assert.Equal(5, r.Value);
    }

    #endregion

    #region NonGeneric Task extensions

    [Fact]
    public async Task T_ThenAsync_Sync_NG_Success()
    {
        var r = await TOkAsync().ThenAsync(AxisResult.Ok);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_ThenAsync_Sync_Typed_Success()
    {
        var r = await TOkAsync().ThenAsync(() => AxisResult.Ok(5));
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Async_NG_Success()
    {
        var r = await TOkAsync().ThenAsync(TOkAsync);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_ThenAsync_Async_Typed_Success()
    {
        var r = await TOkAsync().ThenAsync(() => TOkAsync(7));
        Assert.Equal(7, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Sync_NG_Failure_Skips()
    {
        var called = false;
        await TErrAsync(E1).ThenAsync(() => { called = true; return AxisResult.Ok(); });
        Assert.False(called);
    }

    [Fact]
    public async Task T_TapAsync_Sync()
    {
        var called = false;
        await TOkAsync().TapAsync(() => called = true);
        Assert.True(called);
    }

    [Fact]
    public async Task T_TapAsync_Async()
    {
        var called = false;
        await TOkAsync().TapAsync(() => { called = true; return Task.CompletedTask; });
        Assert.True(called);
    }

    [Fact]
    public async Task T_TapErrorAsync_Sync()
    {
        var captured = 0;
        await TErrAsync(E1).TapErrorAsync(errs => captured = errs.Count);
        Assert.Equal(1, captured);
    }

    [Fact]
    public async Task T_TapErrorAsync_Async()
    {
        var captured = 0;
        await TErrAsync(E1).TapErrorAsync(errs => { captured = errs.Count; return Task.CompletedTask; });
        Assert.Equal(1, captured);
    }

    [Fact]
    public async Task T_MatchAsync_Sync_Success()
    {
        var s = await TOkAsync().MatchAsync(() => "ok", _ => "no");
        Assert.Equal("ok", s);
    }

    [Fact]
    public async Task T_MatchAsync_Async_Success()
    {
        var s = await TOkAsync().MatchAsync(() => Task.FromResult("ok"), _ => Task.FromResult("no"));
        Assert.Equal("ok", s);
    }

    [Fact]
    public async Task T_MatchAsync_Sync_Failure()
    {
        var s = await TErrAsync(E1).MatchAsync(() => "ok", errs => $"c{errs.Count}");
        Assert.Equal("c1", s);
    }

    [Fact]
    public async Task T_MatchAsync_Async_Failure()
    {
        var s = await TErrAsync(E1).MatchAsync(() => Task.FromResult("ok"), errs => Task.FromResult($"c{errs.Count}"));
        Assert.Equal("c1", s);
    }

    #endregion

    #region Generic Task extensions

    [Fact]
    public async Task T_MapAsync_Sync()
    {
        var r = await TOkAsync(5).MapAsync(x => x * 2);
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task T_MapAsync_Async()
    {
        var r = await TOkAsync(5).MapAsync(x => Task.FromResult(x * 2));
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Sync_NG_Preserves_Value()
    {
        var r = await TOkAsync(5).ThenAsync(_ => AxisResult.Ok());
        Assert.True(r.IsSuccess);
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Sync_NG_Failure_Propagates()
    {
        var r = await TOkAsync(5).ThenAsync(_ => AxisResult.Error(E1));
        Assert.True(r.IsFailure);
        Assert.Equal("E1", r.Errors[0].Code);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Sync_Typed()
    {
        var r = await TOkAsync(5).ThenAsync(x => AxisResult.Ok(x.ToString()));
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Async_NG_Preserves_Value()
    {
        var r = await TOkAsync(5).ThenAsync(_ => Task.FromResult(AxisResult.Ok()));
        Assert.True(r.IsSuccess);
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Async_NG_Failure_Propagates()
    {
        var r = await TOkAsync(5).ThenAsync(_ => Task.FromResult(AxisResult.Error(E1)));
        Assert.True(r.IsFailure);
        Assert.Equal("E1", r.Errors[0].Code);
    }

    [Fact]
    public async Task T_ThenAsync_Generic_Async_Typed()
    {
        var r = await TOkAsync(5).ThenAsync(AxisResult.Ok);
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_TapAsync_Generic_Sync_WithValue()
    {
        var captured = 0;
        await TOkAsync(9).TapAsync(x => captured = x);
        Assert.Equal(9, captured);
    }

    [Fact]
    public async Task T_TapAsync_Generic_Async_WithValue()
    {
        var captured = 0;
        await TOkAsync(9).TapAsync(x => { captured = x; return Task.CompletedTask; });
        Assert.Equal(9, captured);
    }

    [Fact]
    public async Task T_TapErrorAsync_Generic_Sync()
    {
        var captured = 0;
        await TErrAsync<int>(E1).TapErrorAsync(errs => captured = errs.Count);
        Assert.Equal(1, captured);
    }

    [Fact]
    public async Task T_TapErrorAsync_Generic_Async()
    {
        var captured = 0;
        await TErrAsync<int>(E1).TapErrorAsync(errs => { captured = errs.Count; return Task.CompletedTask; });
        Assert.Equal(1, captured);
    }

    [Fact]
    public async Task T_EnsureAsync_Predicate_Sync()
    {
        var r = await TOkAsync(10).EnsureAsync(x => x > 0, E1);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_EnsureAsync_Predicate_Async()
    {
        var r = await TOkAsync(10).EnsureAsync(x => Task.FromResult(x > 0), E1);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_EnsureAsync_Validation_Sync()
    {
        var r = await TOkAsync(10).EnsureAsync(_ => AxisResult.Ok());
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_EnsureAsync_Validation_Async()
    {
        var r = await TOkAsync(10).EnsureAsync(_ => Task.FromResult(AxisResult.Ok()));
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_EnsureAsync_Predicate_Sync_Fails()
    {
        var r = await TOkAsync(-1).EnsureAsync(x => x > 0, E1);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_EnsureAsync_Predicate_Async_Fails()
    {
        var r = await TOkAsync(-1).EnsureAsync(x => Task.FromResult(x > 0), E1);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_EnsureAsync_Validation_Sync_Fails()
    {
        var r = await TOkAsync(10).EnsureAsync(_ => AxisResult.Error(E1));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_EnsureAsync_Validation_Async_Fails()
    {
        var r = await TOkAsync(10).EnsureAsync(_ => Task.FromResult(AxisResult.Error(E1)));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_ZipAsync_PureSync()
    {
        var r = await TOkAsync(5).ZipAsync(x => x * 2);
        Assert.Equal((5, 10), r.Value);
    }

    [Fact]
    public async Task T_ZipAsync_PureAsync()
    {
        var r = await TOkAsync(5).ZipAsync(x => Task.FromResult(x * 2));
        Assert.Equal((5, 10), r.Value);
    }

    [Fact]
    public async Task T_ZipAsync_ResultSync()
    {
        var r = await TOkAsync(5).ZipAsync(x => AxisResult.Ok(x + 1));
        Assert.Equal((5, 6), r.Value);
    }

    [Fact]
    public async Task T_ZipAsync_ResultAsync()
    {
        var r = await TOkAsync(5).ZipAsync(x => Task.FromResult(AxisResult.Ok(x + 1)));
        Assert.Equal((5, 6), r.Value);
    }

    [Fact]
    public async Task T_MapErrorAsync_List_Sync()
    {
        var r = await TErrAsync<int>(E1).MapErrorAsync(_ => new[] { E2 }.AsEnumerable());
        Assert.Equal("E2", r.Errors[0].Code);
    }

    [Fact]
    public async Task T_MapErrorAsync_Item_Sync()
    {
        var r = await TErrAsync<int>(E1).MapErrorAsync(e => AxisError.Mapping(e.Code));
        Assert.Equal(AxisErrorType.Mapping, r.Errors[0].Type);
    }

    [Fact]
    public async Task T_MapErrorAsync_Async()
    {
        var r = await TErrAsync<int>(E1).MapErrorAsync(_ => Task.FromResult<IEnumerable<AxisError>>([E2]));
        Assert.Equal("E2", r.Errors[0].Code);
    }

    [Fact]
    public async Task T_RecoverAsync_WithErrors_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverAsync(errs => errs.Count * 10);
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task T_RecoverAsync_WithErrors_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverAsync(errs => Task.FromResult(errs.Count * 10));
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task T_RecoverAsync_NoArgs_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverAsync(() => 42);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_RecoverAsync_NoArgs_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverAsync(() => Task.FromResult(42));
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_RecoverAsync_Default()
    {
        var r = await TErrAsync<int>(E1).RecoverAsync(7);
        Assert.Equal(7, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Predicate_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync(_ => true, _ => 99);
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Predicate_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync(_ => true, _ => Task.FromResult(99));
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Type_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync(AxisErrorType.NotFound, () => 99);
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Type_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync(AxisErrorType.NotFound, () => Task.FromResult(99));
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Code_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync("E1", () => 99);
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverWhenAsync_Code_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverWhenAsync("E1", () => Task.FromResult(99));
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_RecoverNotFoundAsync_Sync()
    {
        var r = await TErrAsync<int>(E1).RecoverNotFoundAsync(() => 42);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_RecoverNotFoundAsync_Async()
    {
        var r = await TErrAsync<int>(E1).RecoverNotFoundAsync(() => Task.FromResult(42));
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_OrElseAsync_Sync()
    {
        var r = await TErrAsync<int>(E1).OrElseAsync(_ => AxisResult.Ok(99));
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_OrElseAsync_Async()
    {
        var r = await TErrAsync<int>(E1).OrElseAsync(_ => Task.FromResult(AxisResult.Ok(99)));
        Assert.Equal(99, r.Value);
    }

    [Fact]
    public async Task T_OrElseAsync_Sync_Combine()
    {
        var r = await TErrAsync<int>(E1).OrElseAsync(_ => AxisResult.Error<int>(E2), combineErrors: true);
        Assert.Equal(2, r.Errors.Count);
    }

    [Fact]
    public async Task T_OrElseAsync_Async_Combine()
    {
        var r = await TErrAsync<int>(E1).OrElseAsync(_ => Task.FromResult(AxisResult.Error<int>(E2)), combineErrors: true);
        Assert.Equal(2, r.Errors.Count);
    }

    [Fact]
    public async Task T_MatchAsync_Generic_Sync_Success()
    {
        var s = await TOkAsync(10).MatchAsync(v => v.ToString(), _ => "x");
        Assert.Equal("10", s);
    }

    [Fact]
    public async Task T_MatchAsync_Generic_Async_Success()
    {
        var s = await TOkAsync(10).MatchAsync(v => Task.FromResult(v.ToString()), _ => Task.FromResult("x"));
        Assert.Equal("10", s);
    }

    [Fact]
    public async Task T_MatchAsync_Generic_Sync_Failure()
    {
        var s = await TErrAsync<int>(E1).MatchAsync(v => v.ToString(), _ => "fail");
        Assert.Equal("fail", s);
    }

    [Fact]
    public async Task T_MatchAsync_Generic_Async_Failure()
    {
        var s = await TErrAsync<int>(E1).MatchAsync(v => Task.FromResult(v.ToString()), _ => Task.FromResult("fail"));
        Assert.Equal("fail", s);
    }

    [Fact]
    public async Task T_SelectManyAsync_Sync()
    {
        var r = await TOkAsync(2).SelectManyAsync(x => AxisResult.Ok(x + 1), (x, y) => x * y);
        Assert.Equal(6, r.Value);
    }

    [Fact]
    public async Task T_SelectManyAsync_Async()
    {
        var r = await TOkAsync(2).SelectManyAsync(x => Task.FromResult(AxisResult.Ok(x + 1)), (x, y) => x * y);
        Assert.Equal(6, r.Value);
    }

    #endregion

    #region Tuple extensions (2-tuple)

    [Fact]
    public async Task T_Tuple2_MapAsync_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.MapAsync((a, b) => a + b);
        Assert.Equal(3, r.Value);
    }

    [Fact]
    public async Task T_Tuple2_MapAsync_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.MapAsync((a, b) => Task.FromResult(a + b));
        Assert.Equal(3, r.Value);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((a, b) => a + b);
        Assert.Equal((1, 2, 3), r.Value);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((a, b) => Task.FromResult(a + b));
        Assert.Equal((1, 2, 3), r.Value);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Result_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((a, b) => AxisResult.Ok(a + b));
        Assert.Equal((1, 2, 3), r.Value);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Result_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((a, b) => Task.FromResult(AxisResult.Ok(a + b)));
        Assert.Equal((1, 2, 3), r.Value);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Sync_Failure()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int)>(E1));
        var r = await t.ZipAsync((a, b) => a + b);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_Async_Failure()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int)>(E1));
        var r = await t.ZipAsync((a, b) => Task.FromResult(a + b));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_ResultSync_Failure_Source()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int)>(E1));
        var r = await t.ZipAsync((a, b) => AxisResult.Ok(a + b));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_ResultSync_Failure_Mapper()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((_, _) => AxisResult.Error<int>(E2));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_ResultAsync_Failure_Source()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int)>(E1));
        var r = await t.ZipAsync((a, b) => Task.FromResult(AxisResult.Ok(a + b)));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple2_ZipAsync_ResultAsync_Failure_Mapper()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2)));
        var r = await t.ZipAsync((_, _) => Task.FromResult(AxisResult.Error<int>(E2)));
        Assert.True(r.IsFailure);
    }

    #endregion

    #region Tuple3 extensions

    [Fact]
    public async Task T_Tuple3_MapAsync_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.MapAsync((a, b, c) => a + b + c);
        Assert.Equal(6, r.Value);
    }

    [Fact]
    public async Task T_Tuple3_MapAsync_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.MapAsync((a, b, c) => Task.FromResult(a + b + c));
        Assert.Equal(6, r.Value);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((a, b, c) => a + b + c);
        Assert.Equal((1, 2, 3, 6), r.Value);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((a, b, c) => Task.FromResult(a + b + c));
        Assert.Equal((1, 2, 3, 6), r.Value);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Result_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((a, b, c) => AxisResult.Ok(a + b + c));
        Assert.Equal((1, 2, 3, 6), r.Value);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Result_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((a, b, c) => Task.FromResult(AxisResult.Ok(a + b + c)));
        Assert.Equal((1, 2, 3, 6), r.Value);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Sync_Failure()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int, int)>(E1));
        var r = await t.ZipAsync((a, b, c) => a + b + c);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_Async_Failure()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int, int)>(E1));
        var r = await t.ZipAsync((a, b, c) => Task.FromResult(a + b + c));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_ResultSync_Failure_Source()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int, int)>(E1));
        var r = await t.ZipAsync((a, b, c) => AxisResult.Ok(a + b + c));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_ResultSync_Failure_Mapper()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((_, _, _) => AxisResult.Error<int>(E2));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_ResultAsync_Failure_Source()
    {
        var t = Task.FromResult(AxisResult.Error<(int, int, int)>(E1));
        var r = await t.ZipAsync((a, b, c) => Task.FromResult(AxisResult.Ok(a + b + c)));
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_Tuple3_ZipAsync_ResultAsync_Failure_Mapper()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3)));
        var r = await t.ZipAsync((_, _, _) => Task.FromResult(AxisResult.Error<int>(E2)));
        Assert.True(r.IsFailure);
    }

    #endregion

    #region Tuple4 extensions

    [Fact]
    public async Task T_Tuple4_MapAsync_Sync()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3, 4)));
        var r = await t.MapAsync((a, b, c, d) => a + b + c + d);
        Assert.Equal(10, r.Value);
    }

    [Fact]
    public async Task T_Tuple4_MapAsync_Async()
    {
        var t = Task.FromResult(AxisResult.Ok((1, 2, 3, 4)));
        var r = await t.MapAsync((a, b, c, d) => Task.FromResult(a + b + c + d));
        Assert.Equal(10, r.Value);
    }

    #endregion

    #region WithValueAsync

    [Fact]
    public async Task T_WithValueAsync_Success_ReturnsNewValue()
    {
        var r = await TOkAsync().WithValueAsync(42);
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_WithValueAsync_Failure_PropagatesErrors()
    {
        var r = await TErrAsync(E1).WithValueAsync(42);
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    #endregion

    #region RequireNotFoundAsync (NonGeneric)

    [Fact]
    public async Task T_RequireNotFoundAsync_NG_Success_ReturnsError()
    {
        var r = await TOkAsync().RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "ALREADY_EXISTS");
    }

    [Fact]
    public async Task T_RequireNotFoundAsync_NG_NotFoundError_ReturnsOk()
    {
        var notFound = AxisError.NotFound("NOT_FOUND");
        var r = await TErrAsync(notFound).RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_RequireNotFoundAsync_NG_OtherError_PropagatesOriginal()
    {
        var r = await TErrAsync(E2).RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E2");
    }

    #endregion

    #region RequireNotFoundAsync (Generic)

    [Fact]
    public async Task T_RequireNotFoundAsync_Generic_Success_ReturnsError()
    {
        var r = await TOkAsync(5).RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "ALREADY_EXISTS");
    }

    [Fact]
    public async Task T_RequireNotFoundAsync_Generic_NotFoundError_ReturnsOk()
    {
        var notFound = AxisError.NotFound("NOT_FOUND");
        var r = await TErrAsync<int>(notFound).RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_RequireNotFoundAsync_Generic_OtherError_PropagatesOriginal()
    {
        var r = await TErrAsync<int>(E2).RequireNotFoundAsync(AxisError.BusinessRule("ALREADY_EXISTS"));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E2");
    }

    #endregion

    #region ActionAsync

    [Fact]
    public async Task T_ActionAsync_Success_Preserves_Value()
    {
        var r = await TOkAsync(5).ActionAsync(_ => Task.FromResult(AxisResult.Ok()));
        Assert.True(r.IsSuccess);
        Assert.Equal(5, r.Value);
    }

    [Fact]
    public async Task T_ActionAsync_InnerFailure_PropagatesError()
    {
        var r = await TOkAsync(5).ActionAsync(_ => Task.FromResult(AxisResult.Error(E1)));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    [Fact]
    public async Task T_ActionAsync_SourceFailure_SkipsAction()
    {
        var called = false;
        var r = await TErrAsync<int>(E1).ActionAsync(_ => { called = true; return Task.FromResult(AxisResult.Ok()); });
        Assert.False(called);
        Assert.True(r.IsFailure);
    }

    #endregion

    #region ThenAsync (Value-Preserving Overload)

    [Fact]
    public async Task T_ThenAsync_ValuePreserving_Success_ReturnsOriginalValue()
    {
        var r = await TOkAsync(42).ThenAsync(_ => Task.FromResult(AxisResult.Ok()));
        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Value);
    }

    [Fact]
    public async Task T_ThenAsync_ValuePreserving_InnerFailure_PropagatesErrors()
    {
        var r = await TOkAsync(42).ThenAsync(_ => Task.FromResult(AxisResult.Error(E1)));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    [Fact]
    public async Task T_ThenAsync_ValuePreserving_OuterFailure_SkipsInner()
    {
        var called = false;
        var r = await TErrAsync<int>(E1).ThenAsync(_ => { called = true; return Task.FromResult(AxisResult.Ok()); });
        Assert.False(called);
        Assert.True(r.IsFailure);
    }

    #endregion

    #region ToAxisResultAsync

    [Fact]
    public async Task T_ToAxisResultAsync_Generic_Async_Success_ReturnsOk()
    {
        var r = await TOkAsync(5).ToAxisResultAsync(async x => AxisResult.Ok());
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Generic_Async_PropagatesError()
    {
        var r = await TOkAsync(5).ToAxisResultAsync(async x => AxisResult.Error(E1));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Async_OuterFailure_SkipsInner()
    {
        var called = false;
        var r = await TErrAsync<int>(E1).ToAxisResultAsync(async x => { called = true; return AxisResult.Ok(); });
        Assert.False(called);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Generic_Sync_Success_ReturnsOk()
    {
        var r = await TOkAsync(5).ToAxisResultAsync(AxisResult.Ok);
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Generic_Sync_PropagatesError()
    {
        var r = await TOkAsync(5).ToAxisResultAsync(x => AxisResult.Error(E1));
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Sync_OuterFailure_SkipsInner()
    {
        var called = false;
        var r = await TErrAsync<int>(E1).ToAxisResultAsync(x => { called = true; return AxisResult.Ok(); });
        Assert.False(called);
        Assert.True(r.IsFailure);
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Parameterless_Generic_Success_ReturnsOk()
    {
        var r = await TOkAsync(5).ToAxisResultAsync();
        Assert.True(r.IsSuccess);
    }

    [Fact]
    public async Task T_ToAxisResultAsync_Parameterless_Generic_Failure_PropagatesErrors()
    {
        var r = await TErrAsync<int>(E1).ToAxisResultAsync();
        Assert.True(r.IsFailure);
        Assert.Contains(r.Errors, e => e.Code == "E1");
    }

    #endregion
}
