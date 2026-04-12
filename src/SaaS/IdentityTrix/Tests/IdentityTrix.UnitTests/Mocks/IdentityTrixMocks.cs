using AxisTrix;
using AxisTrix.Caching;
using IdentityTrix.Application.ExternalApis;
using IdentityTrix.Ports;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IdentityTrix.UnitTests.Mocks;

internal record IdentityTrixMocks
{
    public static IdentityTrixMocks CreateSuccessfulMocks()
    {
        var mocks = new IdentityTrixMocks();

        mocks.UowProvider.Setup(x => x.UnitOfWork.SaveChangesAsync())
            .ReturnsAsync(AxisResult.Ok());

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
