using AxisTrix.DependencyInjection;
using TenantTrix.Contracts.Authentication.v1;
using TenantTrix.Contracts.ExternalApis.v1;
using Microsoft.Extensions.DependencyInjection;
using TenantTrix.Application;
using TenantTrix.Sdk.Application.Authentication.v1;
using TenantTrix.Sdk.Application.ExternalApis.v1;

namespace TenantTrix.Sdk.Application;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddTenantTrixSdkApplication(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IExternalApisMediator, ExternalApisMediator>();
        builder.Services.AddScoped<IAuthenticationMediator, AuthenticationMediator>();
        return builder.AddTenantTrixApplication();
    }
}
