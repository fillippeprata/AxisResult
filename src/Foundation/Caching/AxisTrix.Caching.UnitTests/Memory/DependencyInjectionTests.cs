using AxisTrix.Accessor;
using AxisTrix.Caching.Memory;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Caching.UnitTests.Memory;

public class DependencyInjectionTests
{
    [Fact]
    public void AddMemoryCacheTrix_ShouldRegisterICacheTrixAsMemoryCacheAdapter()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        builder.AddAxisMemoryCache();
        builder.Services.AddSingleton<IAxisMediatorAccessor>(_ =>
        {
            var mediator = new Mock<IAxisMediator>();
            mediator.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
            return new AxisMediatorAccessor
            {
                AxisMediator = mediator.Object
            };
        });
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var cache = serviceProvider.GetService<IAxisCache>();
        Assert.NotNull(cache);
        Assert.IsType<MemoryCacheAdapter>(cache);
    }
}
