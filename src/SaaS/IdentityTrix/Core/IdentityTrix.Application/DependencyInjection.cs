using System.Reflection;
using AxisTrix.CQRS;
using AxisTrix.DependencyInjection;
using IdentityTrix.Application.Authentication;
using IdentityTrix.Application.ExternalApis;

namespace IdentityTrix.Application;

internal static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixApplication(this ServiceCollectionBuilder builder)
    {
        return builder
            .AddAuthenticationModule()
            .AddExternalApisModule()
            .AddCqrsMediator(Assembly.GetExecutingAssembly());
    }
}
