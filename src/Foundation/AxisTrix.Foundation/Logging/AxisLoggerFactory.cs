using AxisTrix.Accessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AxisTrix.Logging;

internal static class AxisLoggerFactory
{
    public static ILoggerFactory Create(IServiceProvider serviceProvider)
    {
        var accessor = serviceProvider.GetService<IAxisMediatorAccessor>();
        var mediator = accessor?.AxisMediator;

        if (mediator is null)
        {
            //todo: Implement the factory for the default singleTon factory
            return LoggerFactory.Create(builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug));
        }

        //todo: Implement the factory for the tenant regarding tenant configs
        Console.WriteLine($"Factory for the tenant with correlation id: {mediator.TraceId}");
        return LoggerFactory.Create(builder => builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug));
    }
}
