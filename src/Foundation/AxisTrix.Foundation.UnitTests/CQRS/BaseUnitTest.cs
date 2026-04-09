using System.Reflection;
using AxisTrix.Accessor;
using AxisTrix.CQRS;
using AxisTrix.DependencyInjection;
using AxisTrix.Telemetry;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Mediator.UnitTests.CQRS;

public class BaseUnitTest
{
    protected static IServiceProvider DefaultServiceProvider()
    {
        var serviceProvider = new ServiceCollection()
            . InitAxisTrixAdd()
            .AddOpenTelemetryAxis()
            .AddCqrsMediator(Assembly.GetExecutingAssembly())
            .EndAxisTrixAdd()
            .BuildServiceProvider();
        var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
        contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
        contextAccessor.PersonId = Guid.CreateVersion7().ToString();
        return serviceProvider;
    }

}
