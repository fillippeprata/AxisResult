using System.Reflection;
using Axis;
using AxisMediator.Contracts;
using AxisMediator.CQRS;
using AxisValidator.FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AxisMediator.UnitTests.CQRS;

public class BaseUnitTest
{
    protected static IServiceProvider DefaultServiceProvider()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var serviceProvider = ServiceCollectionServiceExtensions.AddSingleton(new ServiceCollection()
                .AddCqrsMediator(assembly)
                .AddAxisValidator(assembly)
                .AddAxisLogger()
                .AddLoggingBehavior(), AxisLoggerFactory.Create)
            .AddAxisMediator()
            .AddPerformanceBehavior()
            .BuildServiceProvider();

        var contextAccessor = serviceProvider.GetRequiredService<IAxisMediatorContextAccessor>();
        contextAccessor.OriginId =  $"ExternalApiTrix-{Guid.NewGuid():N}";
        contextAccessor.PersonId = Guid.CreateVersion7().ToString();
        return serviceProvider;
    }

}
