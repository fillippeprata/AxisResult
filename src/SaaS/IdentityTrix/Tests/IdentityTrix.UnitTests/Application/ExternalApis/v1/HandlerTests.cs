using AxisTrix.Results;
using IdentityTrix.Contracts.ExternalApis.v1;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.UnitTests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IdentityTrix.UnitTests.Application.ExternalApis.v1;

public class HandlerTests
{
    private static IExternalApisMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

    [Fact]
    public async Task AddHandlerShouldReturnSuccessWhenCommandIsValidAsync()
    {
        //Arrange
        #region IdentityTrixMocks

        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        using var scope = services.GetServiceProvider().CreateScope();

        #endregion

        var mediator = Mediator(scope);
        var command = new AddExternalApiCommand { ApiName = "test-api-name" };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Secret);
        Assert.NotEmpty(result.Value.Name);
        mocks.ExternalApiWriter.Verify(x => x.CreateAsync(It.IsAny<IExternalApiEntityProperties>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdHandlerShouldReturnSuccessWhenExternalApiExistsAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        const string testName = "api-name-abc";
        const string testSecret = "api-secret-123";

        #region IdentityTrixMocks

        var mocks = IdentityTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, testSecret, testName)));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        #endregion

        var mediator = Mediator(scope);
        var query = new GetExternalApiByIdQuery { ExternalApiId = externalApiId };

        //Act
        var result = await mediator.GetByIdAsync(query);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(externalApiId.ToString(), result.Value.ExternalApiId);
        Assert.Equal(testName, result.Value.Name);
    }

    private class MockExternalApiProperties(ExternalApiId externalApiId, string secret, string name) : IExternalApiEntityProperties
    {
        public ExternalApiId ExternalApiId { get; } = externalApiId;
        public string HashedSecret { get; } = secret;
        public string ApiName { get; } = name;
    }
}
