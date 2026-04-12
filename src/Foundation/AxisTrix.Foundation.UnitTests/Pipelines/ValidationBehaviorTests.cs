using AxisResult;
using AxisTrix.CQRS;
using AxisTrix.Pipelines;
using AxisTrix.Validation;

namespace AxisTrix.Mediator.UnitTests.Pipelines;

public class ValidationBehaviorTests
{
    private record TestCommand : IAxisRequest;
    private record TestResponse : IAxisResponse;

    private class SuccessValidator : IAxisValidator<TestCommand>
    {
        public AxisResult.AxisResult Validate(TestCommand instance) => AxisResult.AxisResult.Ok();
        public Task<AxisResult.AxisResult> ValidateAsync(TestCommand instance) => Task.FromResult(AxisResult.AxisResult.Ok());
    }

    private class FailureValidator : IAxisValidator<TestCommand>
    {
        public AxisResult.AxisResult Validate(TestCommand instance) => AxisError.ValidationRule("INVALID_COMMAND");
        public Task<AxisResult.AxisResult> ValidateAsync(TestCommand instance) => Task.FromResult<AxisResult.AxisResult>(AxisError.ValidationRule("INVALID_COMMAND"));
    }

    private class StubServiceProvider(IAxisValidator<TestCommand>? validator) : IServiceProvider
    {
        public object? GetService(Type serviceType)
            => serviceType == typeof(IAxisValidator<TestCommand>) ? validator : null;
    }

    // ── IPipelineBehavior<TRequest> (non-generic / void) ────────────────────

    [Fact]
    public async Task NonGeneric_WhenValidationPasses_CallsNext()
    {
        var behavior = new ValidationBehavior<TestCommand>(new StubServiceProvider(new SuccessValidator()));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok());
        });

        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task NonGeneric_WhenValidationFails_DoesNotCallNext()
    {
        var behavior = new ValidationBehavior<TestCommand>(new StubServiceProvider(new FailureValidator()));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok());
        });

        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task NonGeneric_WhenNoValidatorRegistered_CallsNext()
    {
        var behavior = new ValidationBehavior<TestCommand>(new StubServiceProvider(null));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok());
        });

        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
    }

    // ── IPipelineBehavior<TRequest, TResponse> (generic) ────────────────────

    [Fact]
    public async Task Generic_WhenValidationPasses_CallsNext()
    {
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(new StubServiceProvider(new SuccessValidator()));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok(new TestResponse()));
        });

        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Generic_WhenValidationFails_DoesNotCallNext()
    {
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(new StubServiceProvider(new FailureValidator()));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok(new TestResponse()));
        });

        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Generic_WhenValidationFails_PropagatesErrors()
    {
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(new StubServiceProvider(new FailureValidator()));

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
            Task.FromResult(AxisResult.AxisResult.Ok(new TestResponse())));

        Assert.Single(result.Errors);
        Assert.Equal("INVALID_COMMAND", result.Errors[0].Code);
    }

    [Fact]
    public async Task Generic_WhenNoValidatorRegistered_CallsNext()
    {
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(new StubServiceProvider(null));
        var nextCalled = false;

        var result = await behavior.HandleAsync(new TestCommand(), new(), () =>
        {
            nextCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok(new TestResponse()));
        });

        Assert.True(nextCalled);
        Assert.True(result.IsSuccess);
    }
}
