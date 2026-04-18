using Axis;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using Microsoft.Extensions.DependencyInjection;
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
}
