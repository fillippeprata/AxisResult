using Axis;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TenantTrix.Application.ExternalApis;
using TenantTrix.Ports;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.SharedKernel.ExternalApis;

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

    public Mock<IAxisCache> Cache { get; init; } = new();
}
