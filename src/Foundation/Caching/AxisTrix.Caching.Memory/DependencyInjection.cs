using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Caching.Memory;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddAxisMemoryCache(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IAxisCache, MemoryCacheAdapter>();
        return builder;
    }
}
