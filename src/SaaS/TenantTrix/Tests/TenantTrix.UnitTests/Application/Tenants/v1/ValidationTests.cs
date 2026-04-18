using Axis;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Contracts.Tenants.v1;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Contracts.Tenants.v1.EditTenant;
using TenantTrix.UnitTests.Mocks;

namespace TenantTrix.UnitTests.Application.Tenants.v1;

public class ValidationTests
{
    private static ITenantsMediator Mediator(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<ITenantsMediator>();

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = null };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameIsEmptyAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = "" };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameIsWhitespaceAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = "   " };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameContainsSpaceAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = "some name" };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameContainsSpecialCharactersAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = "acme@corp!" };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task AddShouldReturnValidationErrorWhenTenantNameIsTooLongAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new AddTenantCommand { TenantName = new string('a', 256) };

        //Act
        var result = await mediator.AddAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantIdIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = null, TenantName = "some-name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantIdIsNotGuidV7Async()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.NewGuid().ToString(), TenantName = "some-name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_ID_NULL_OR_NOT_GUID_7", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantNameIsNullAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.CreateVersion7().ToString(), TenantName = null };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantNameIsEmptyAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.CreateVersion7().ToString(), TenantName = "" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantNameContainsSpaceAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.CreateVersion7().ToString(), TenantName = "bad name" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantNameContainsSpecialCharactersAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.CreateVersion7().ToString(), TenantName = "acme#corp" };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }

    [Fact]
    public async Task EditShouldReturnValidationErrorWhenTenantNameIsTooLongAsync()
    {
        //Arrange
        using var scope = TenantTrixMocks.GetServiceProvider().CreateScope();
        var mediator = Mediator(scope);
        var command = new EditTenantCommand { TenantId = Guid.CreateVersion7().ToString(), TenantName = new string('a', 256) };

        //Act
        var result = await mediator.EditAsync(command);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, x => x is { Code: "TENANT_NAME_INVALID", Type: AxisErrorType.ValidationRule });
    }
}
