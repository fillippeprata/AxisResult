using AxisTrix.Results;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.UnitTests.Mocks;
using IndentityTrix.Contracts.Authentication.v1;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IdentityTrix.UnitTests.Application.Authentication.v1;

public class HandlerTests
{
    private static IAuthenticationMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IAuthenticationMediator>();

    [Fact]
    public async Task AuthenticateHandlerShouldReturnSuccessWhenSecretIsValidAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var plainSecret = ExternalApiSecret.Generate();
        var hashedSecret = ExternalApiSecret.Hash(plainSecret);

        var mocks = IdentityTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetExternalApiByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, hashedSecret, "test-api")));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = externalApiId, Secret = plainSecret };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task AuthenticateHandlerShouldReturnFailureWhenSecretIsInvalidAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var storedSecret = ExternalApiSecret.Hash(ExternalApiSecret.Generate());
        var wrongPlainSecret = ExternalApiSecret.Generate();

        var mocks = IdentityTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetExternalApiByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, storedSecret, "test-api")));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = externalApiId, Secret = wrongPlainSecret };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "INVALID_EXTERNAL_API_ID_OR_SECRET");
    }

    [Fact]
    public async Task AuthenticateHandlerShouldReturnFailureWhenExternalApiNotFoundAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;

        var mocks = IdentityTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetExternalApiByIdAsync(externalApiId))
            .ReturnsAsync(AxisError.NotFound("EXTERNAL_API_NOT_FOUND"));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new AuthenticateExternalApiCommand { ExternalApiId = externalApiId, Secret = "some-secret" };

        //Act
        var result = await mediator.AuthenticateAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "EXTERNAL_API_NOT_FOUND");
    }

    private class MockExternalApiProperties(ExternalApiId externalApiId, string hashedSecret, string apiName) : IExternalApiEntityProperties
    {
        public ExternalApiId ExternalApiId { get; } = externalApiId;
        public string HashedSecret { get; } = hashedSecret;
        public string ApiName { get; } = apiName;
    }
}
