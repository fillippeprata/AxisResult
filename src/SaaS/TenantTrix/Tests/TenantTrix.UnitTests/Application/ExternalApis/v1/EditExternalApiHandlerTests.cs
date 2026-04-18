using Axis;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;
using TenantTrix.SharedKernel.ExternalApis;
using TenantTrix.UnitTests.Mocks;

namespace TenantTrix.UnitTests.Application.ExternalApis.v1;

public class EditExternalApiHandlerTests
{
    private static IExternalApisMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IExternalApisMediator>();

    [Fact]
    public async Task EditHandlerShouldReturnSuccessWhenExternalApiExistsAndNameIsUniqueAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var tenantId = TenantId.New;
        const string newName = "renamed-api";

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, "hashed", "old-name", tenantId)));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = externalApiId, ApiName = newName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(externalApiId.ToString(), result.Value.ExternalApiId);
        Assert.Equal(newName, result.Value.ApiName);
        mocks.ExternalApiWriter.Verify(x => x.UpdateNameAsync(externalApiId, newName), Times.Once);
    }

    [Fact]
    public async Task EditHandlerShouldReturnSuccessWhenNameIsSameAsCurrentAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var tenantId = TenantId.New;
        const string currentName = "same-name";

        var currentApi = new MockExternalApiProperties(externalApiId, "hashed", currentName, tenantId);

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(currentApi));
        mocks.ExternalApiReader.Setup(x => x.GetByNameAsync(currentName))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(currentApi));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = externalApiId, ApiName = currentName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        mocks.ExternalApiWriter.Verify(x => x.UpdateNameAsync(externalApiId, currentName), Times.Once);
    }

    [Fact]
    public async Task EditHandlerShouldReturnFailureWhenNameIsTakenByAnotherExternalApiAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;
        var anotherApiId = ExternalApiId.New;
        var tenantId = TenantId.New;
        const string conflictingName = "taken-name";

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(externalApiId))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(externalApiId, "hashed", "old-name", tenantId)));
        mocks.ExternalApiReader.Setup(x => x.GetByNameAsync(conflictingName))
            .ReturnsAsync(AxisResult.Ok<IExternalApiEntityProperties>(new MockExternalApiProperties(anotherApiId, "other-hashed", conflictingName, tenantId)));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = externalApiId, ApiName = conflictingName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "EXTERNAL_API_NAME_ALREADY_EXISTS");
        mocks.ExternalApiWriter.Verify(x => x.UpdateNameAsync(It.IsAny<ExternalApiId>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditHandlerShouldReturnFailureWhenExternalApiNotFoundAsync()
    {
        //Arrange
        var externalApiId = ExternalApiId.New;

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.ExternalApiReader.Setup(x => x.GetByIdAsync(It.IsAny<ExternalApiId>()))
            .ReturnsAsync(AxisError.NotFound("EXTERNAL_API_NOT_FOUND"));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = externalApiId, ApiName = "any-name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "EXTERNAL_API_NOT_FOUND");
        mocks.ExternalApiWriter.Verify(x => x.UpdateNameAsync(It.IsAny<ExternalApiId>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenExternalApiIdIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = null, ApiName = "name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenApiNameIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), ApiName = null };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenApiNameIsEmptyAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), ApiName = "" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenApiNameContainsSpaceAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), ApiName = "some name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenApiNameContainsSpecialCharactersAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditExternalApiCommand { ExternalApiId = Guid.CreateVersion7().ToString(), ApiName = "api@name!" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "EXTERNAL_API_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    private class MockExternalApiProperties(ExternalApiId externalApiId, string secret, string name, TenantId tenantId) : IExternalApiEntityProperties
    {
        public ExternalApiId ExternalApiId { get; } = externalApiId;
        public string HashedSecret { get; } = secret;
        public string ApiName { get; } = name;
        public TenantId TenantId { get; } = tenantId;
    }
}
