using Axis;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TenantTrix.Application.ExternalApis;
using TenantTrix.Ports;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.Ports.Tenants;
using TenantTrix.SharedKernel.ExternalApis;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.UnitTests.Mocks;

internal record TenantTrixMocks
{
    public static TenantTrixMocks CreateSuccessfulMocks()
    {
        var mocks = new TenantTrixMocks();

        mocks.UowProvider.Setup(x => x.UnitOfWork.SaveChangesAsync())
            .ReturnsAsync(AxisResult.Ok());

        mocks.ExternalApiReader.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Error<IExternalApiEntityProperties>(AxisError.NotFound("EXTERNAL_API_NOT_FOUND")));

        mocks.ExternalApiWriter.Setup(x => x.CreateAsync(It.IsAny<IExternalApiEntityProperties>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.ExternalApiWriter.Setup(x => x.UpdateNameAsync(It.IsAny<ExternalApiId>(), It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.ExternalApiWriter.Setup(x => x.UpdateSecretAsync(It.IsAny<ExternalApiId>(), It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.TenantReader.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Error<ITenantEntityProperties>(AxisError.NotFound("TENANT_NOT_FOUND")));

        mocks.TenantWriter.Setup(x => x.CreateAsync(It.IsAny<ITenantEntityProperties>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.TenantWriter.Setup(x => x.UpdateNameAsync(It.IsAny<TenantId>(), It.IsAny<string>()))
            .ReturnsAsync(AxisResult.Ok());

        mocks.Cache.Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<AxisResult<IExternalApiAggregateApplication>>>>(),
                It.IsAny<TimeSpan?>()))
            .Returns((string _, Func<Task<AxisResult<IExternalApiAggregateApplication>>> factory, TimeSpan? _) => factory());

        return mocks;
    }

    public static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddMocks(CreateSuccessfulMocks());
        return services.GetServiceProvider();
    }

    public Mock<IUnitOfWorkProvider> UowProvider { get; init; } = new();
    public Mock<IExternalApisWritePort> ExternalApiWriter { get; init; } = new();
    public Mock<IExternalApisReaderPort> ExternalApiReader { get; init; } = new();
    public Mock<ITenantsWritePort> TenantWriter { get; init; } = new();
    public Mock<ITenantsReaderPort> TenantReader { get; init; } = new();

    public Mock<IAxisCache> Cache { get; init; } = new();
}
