using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Ports.Tenants;

namespace TenantTrix.Driven.Repositories.Postgres.Tenants;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddTenantRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<TenantsRepository>();
        builder.Services.AddScoped<ITenantsReaderPort>(sp => sp.GetRequiredService<TenantsRepository>());
        builder.Services.AddScoped<ITenantsWritePort>(sp => sp.GetRequiredService<TenantsRepository>());
        return builder;
    }
}
