using Axis;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.AuthenticateExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using TenantTrix.UnitTests.Mocks;

namespace TenantTrix.UnitTests.Application.ExternalApis.v1;

public class ValidationTests
{
    private static IExternalApisMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

    [Fact]
    public async Task ShouldReturnValidationRuleErrorWhenExternalApiIdIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var query = new GetExternalApiByIdQuery { ExternalApiId = null };

        //Act
        var result = await mediator.GetByIdAsync(query);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationRuleErrorWhenExternalApiIdIsNotGuid7Async()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var query = new GetExternalApiByIdQuery { ExternalApiId = Guid.NewGuid().ToString() };

        //Act
        var result = await mediator.GetByIdAsync(query);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenExternalApiIdIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = null!, Secret = "some-secret" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenExternalApiIdIsNotAGuidAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = "not-a-guid", Secret = "some-secret" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenExternalApiIdIsNotGuidV7Async()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = Guid.NewGuid().ToString(), Secret = "some-secret" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenSecretIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), Secret = null! };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_SECRET_NULL_OR_EMPTY", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenSecretIsEmptyAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), Secret = "" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_SECRET_NULL_OR_EMPTY", Type: AxisErrorType.ValidationRule });
    }
}
