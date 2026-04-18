using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TenantTrix.Application.Tenants;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddTenantsModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ITenantAggregateApplicationFactory, TenantAggregateApplicationFactory>();
        return builder;
    }
}
