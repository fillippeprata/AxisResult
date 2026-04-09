using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using AxisTrix.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace AxisTrix.Mediator.UnitTests.CQRS;

public class CommandHandlerTests: BaseUnitTest
{
    public record TestCommand : IAxisCommand<TestResponse>
    {
        public bool? Ping { get; init; }
    }

    public class TestValidator : AxisValidatorBase<TestCommand>
    {
        public TestValidator()
        {
            NotNullOrEmpty(x => x.Ping, "PING_NULL_OR_EMPTY");
        }
    }

    public record TestResponse : IAxisCommandResponse
    {
        public required bool Pong { get; init; }
    }

    public class TestHandler : IAxisCommandHandler<TestCommand, TestResponse>
    {
        public Task<AxisResult<TestResponse>> HandleAsync(TestCommand command)
            => Task.FromResult<AxisResult<TestResponse>>(
                command.Ping!.Value
                    ? new TestResponse { Pong = true }
                    : throw new TestClassException("ping is null or false"));
    }

    [Fact]
    public async Task ShouldReturnValidationRuleErrorWhenExternalApiIdIsNullAsync()
    {
        //Arrange
        using var scope = DefaultServiceProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IAxisMediator>();

        //Act
        var result = await mediator.Cqrs.ExecuteAsync<TestCommand, TestResponse>(new TestCommand() { Ping = true });

        //Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Match(onSuccess: () => true, onFailure: _ => false));
    }

}
