using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Bus.Memory;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddMemoryBusTrix(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddSingleton<IAxisBus, MemoryBusAdapter>();
        return builder;
    }
}
