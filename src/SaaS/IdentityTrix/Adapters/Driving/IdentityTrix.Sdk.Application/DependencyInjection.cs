using AxisTrix.DependencyInjection;
using IdentityTrix.Application;
using IdentityTrix.Sdk.Application.Authentication.v1;
using IdentityTrix.Sdk.Application.ExternalApis.v1;
using IndentityTrix.Contracts.Authentication.v1;
using IndentityTrix.Contracts.ExternalApis.v1;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityTrix.Sdk.Application;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixSdkApplication(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IExternalApisMediator, ExternalApisMediator>();
        builder.Services.AddScoped<IAuthenticationMediator, AuthenticationMediator>();
        return builder.AddIdentityTrixApplication();
    }
}
