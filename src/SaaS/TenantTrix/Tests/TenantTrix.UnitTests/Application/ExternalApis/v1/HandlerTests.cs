using Axis;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TenantTrix.SharedKernel.ExternalApis;
using TenantTrix.UnitTests.Mocks;

namespace TenantTrix.UnitTests.Application.ExternalApis.v1;

public class HandlerTests
{
    private static IExternalApisMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

    [Fact]
    public async Task AddHandlerShouldReturnSuccessWhenCommandIsValidAsync()
    {
        //Arrange
        #region TenantTrixMocks

        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        using var scope = ServiceProviderServiceExtensions.CreateScope(services.GetServiceProvider());

        #endregion

        var mediator = Mediator(scope);
        var command = new AddExternalApiCommand { ApiName = "test-api-name", TenantId = TenantId.New.ToString() };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Secret);
        Assert.NotEmpty(result.Value.Name);
        Assert.NotEmpty(result.Value.TenantId);
        mocks.ExternalApiWriter.Verify(x => x.CreateAsync(It.IsAny<IExternalApiEntityProperties>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdHandlerShouldReturnSuccessWhenExternalApiExistsAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var tenantId = TenantId.New;
        const string testName = "api-name-abc";
        const string testSecret = "api-secret-123";

        #region TenantTrixMocks

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, testSecret, testName, tenantId)));

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
        Assert.Equal(tenantId.ToString(), result.Value.TenantId);
    }

    private class MockExternalApiProperties(ExternalApiId externalApiId, string secret, string name, TenantId tenantId) : IExternalApiEntityProperties
    {
        public ExternalApiId ExternalApiId { get; } = externalApiId;
        public string HashedSecret { get; } = secret;
        public string ApiName { get; } = name;
        public TenantId TenantId { get; } = tenantId;
    }
}
