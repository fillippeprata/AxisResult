using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Application;
using TenantTrix.Contracts.ExternalApis.v1;
using TenantTrix.Sdk.Application.ExternalApis.v1;

namespace TenantTrix.Sdk.Application;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddTenantTrixSdkApplication(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IExternalApisMediator, ExternalApisMediator>();
        return builder.AddTenantTrixApplication();
    }
}
