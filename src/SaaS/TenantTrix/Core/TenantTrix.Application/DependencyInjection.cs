using System.Reflection;
using AxisMediator.CQRS;
using AxisTrix.DependencyInjection;
using AxisValidator.FluentValidation;
using TenantTrix.Application.ExternalApis;
using TenantTrix.Application.Tenants;

namespace TenantTrix.Application;

internal static class DependencyInjection
{
    public static ServiceCollectionBuilder AddTenantTrixApplication(this ServiceCollectionBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.Services
            .AddCqrsMediator(assembly)
            .AddAxisValidator(assembly);

        return builder
            .AddExternalApisModule()
            .AddTenantsModule();

    }
}
