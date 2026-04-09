using AxisTrix.Results;
using IdentityTrix.UnitTests.Mocks;
using IndentityTrix.Contracts.Authentication.v1;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.UnitTests.Application.Authentication.v1;

public class ValidationTests
{
    private static IAuthenticationMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IAuthenticationMediator>();

    [Fact]
    public async Task ShouldReturnValidationErrorWhenExternalApiIdIsNullAsync()
    {
        //Arrange
        using var scope = IdentityTrixMocks.GetServiceProvider().CreateScope();
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
        using var scope = IdentityTrixMocks.GetServiceProvider().CreateScope();
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
        using var scope = IdentityTrixMocks.GetServiceProvider().CreateScope();
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
        using var scope = IdentityTrixMocks.GetServiceProvider().CreateScope();
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
        using var scope = IdentityTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), Secret = "" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_SECRET_NULL_OR_EMPTY", Type: AxisErrorType.ValidationRule });
    }
}
