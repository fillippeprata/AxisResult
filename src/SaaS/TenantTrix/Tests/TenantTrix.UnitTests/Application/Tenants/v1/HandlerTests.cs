using Axis;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TenantTrix.Contracts.Tenants.v1;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Contracts.Tenants.v1.EditTenant;
using TenantTrix.SharedKernel.Tenants;
using TenantTrix.UnitTests.Mocks;

namespace TenantTrix.UnitTests.Application.Tenants.v1;

public class HandlerTests
{
    private static ITenantsMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<ITenantsMediator>();

    [Fact]
    public async Task AddHandlerShouldReturnSuccessWhenCommandIsValidAsync()
    {
        //Arrange
        var services = new ServiceCollection();
        var mocks = services.AddSuccessfulMocks();
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = "test-tenant-name" };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.TenantId);
        Assert.Equal(command.TenantName, result.Value.TenantName);
        mocks.TenantWriter.Verify(x => x.CreateAsync(It.IsAny<ITenantEntityProperties>()), Times.Once);
    }

    [Fact]
    public async Task AddHandlerShouldReturnFailureWhenTenantNameAlreadyExistsAsync()
    {
        //Arrange
        const string existingName = "existing-tenant";

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.TenantReader.Setup(x => x.GetByNameAsync(existingName))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(new MockTenantProperties(TenantId.New, existingName)));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = existingName };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "TENANT_NAME_ALREADY_EXISTS");
        mocks.TenantWriter.Verify(x => x.CreateAsync(It.IsAny<ITenantEntityProperties>()), Times.Never);
    }

    [Fact]
    public async Task EditHandlerShouldReturnSuccessWhenTenantExistsAndNameIsUniqueAsync()
    {
        //Arrange
        var tenantId = TenantId.New;
        const string newName = "renamed-tenant";

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.TenantReader.Setup(x => x.GetByIdAsync(tenantId))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(new MockTenantProperties(tenantId, "old-name")));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = tenantId, TenantName = newName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(tenantId.ToString(), result.Value.TenantId);
        Assert.Equal(newName, result.Value.TenantName);
        mocks.TenantWriter.Verify(x => x.UpdateNameAsync(tenantId, newName), Times.Once);
    }

    [Fact]
    public async Task EditHandlerShouldReturnSuccessWhenNameIsSameAsCurrentTenantAsync()
    {
        //Arrange
        var tenantId = TenantId.New;
        const string currentName = "same-name";

        var currentTenant = new MockTenantProperties(tenantId, currentName);

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.TenantReader.Setup(x => x.GetByIdAsync(tenantId))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(currentTenant));
        mocks.TenantReader.Setup(x => x.GetByNameAsync(currentName))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(currentTenant));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = tenantId, TenantName = currentName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsSuccess);
        mocks.TenantWriter.Verify(x => x.UpdateNameAsync(tenantId, currentName), Times.Once);
    }

    [Fact]
    public async Task EditHandlerShouldReturnFailureWhenNameIsTakenByAnotherTenantAsync()
    {
        //Arrange
        var tenantId = TenantId.New;
        var anotherTenantId = TenantId.New;
        const string conflictingName = "taken-name";

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.TenantReader.Setup(x => x.GetByIdAsync(tenantId))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(new MockTenantProperties(tenantId, "old-name")));
        mocks.TenantReader.Setup(x => x.GetByNameAsync(conflictingName))
            .ReturnsAsync(AxisResult.Ok<ITenantEntityProperties>(new MockTenantProperties(anotherTenantId, conflictingName)));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = tenantId, TenantName = conflictingName };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "TENANT_NAME_ALREADY_EXISTS");
        mocks.TenantWriter.Verify(x => x.UpdateNameAsync(It.IsAny<TenantId>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditHandlerShouldReturnFailureWhenTenantNotFoundAsync()
    {
        //Arrange
        var tenantId = TenantId.New;

        var mocks = TenantTrixMocks.CreateSuccessfulMocks();
        mocks.TenantReader.Setup(x => x.GetByIdAsync(It.IsAny<TenantId>()))
            .ReturnsAsync(AxisError.NotFound("TENANT_NOT_FOUND"));

        var services = new ServiceCollection();
        services.AddMocks(mocks);
        using var scope = services.GetServiceProvider().CreateScope();

        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = tenantId, TenantName = "any-name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x.Code == "TENANT_NOT_FOUND");
        mocks.TenantWriter.Verify(x => x.UpdateNameAsync(It.IsAny<TenantId>(), It.IsAny<string>()), Times.Never);
    }

    private class MockTenantProperties(TenantId tenantId, string tenantName) : ITenantEntityProperties
    {
        public TenantId TenantId { get; } = tenantId;
        public string TenantName { get; } = tenantName;
    }
}
