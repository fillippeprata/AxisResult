using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.DependencyInjection;

public class ServiceCollectionBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
}