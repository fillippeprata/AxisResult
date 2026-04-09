using AxisTrix.Bus.Memory;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Bus.UnitTests.Memory;

public class DependencyInjectionTests
{
    [Fact]
    public void AddMemoryBusTrix_ShouldRegisterIAxisBusAsMemoryBusAdapter()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        builder.AddMemoryBusTrix();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var bus = serviceProvider.GetService<IAxisBus>();
        Assert.NotNull(bus);
        Assert.IsType<MemoryBusAdapter>(bus);
    }

    [Fact]
    public void AddMemoryBusTrix_ShouldRegisterAsSingleton()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        builder.AddMemoryBusTrix();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var instance1 = serviceProvider.GetService<IAxisBus>();
        var instance2 = serviceProvider.GetService<IAxisBus>();
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void AddMemoryBusTrix_ShouldReturnBuilderForChaining()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        var result = builder.AddMemoryBusTrix();

        // Assert
        Assert.Same(builder, result);
    }
}
