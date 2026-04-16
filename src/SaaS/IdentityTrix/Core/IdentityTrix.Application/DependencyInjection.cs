using System.Reflection;
using AxisMediator.CQRS;
using AxisTrix.DependencyInjection;
using AxisValidator.FluentValidation;
using IdentityTrix.Application.Authentication;
using IdentityTrix.Application.ExternalApis;

namespace IdentityTrix.Application;

internal static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixApplication(this ServiceCollectionBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.Services
            .AddCqrsMediator(assembly)
            .AddAxisValidator(assembly);

        return builder
            .AddAuthenticationModule()
            .AddExternalApisModule();

    }
}
