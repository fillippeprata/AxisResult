using System.Reflection;
using AxisMediator.CQRS;
using AxisTrix.DependencyInjection;
using AxisValidator.FluentValidation;
using DataPrivacyTrix.Application.Cellphones;

namespace DataPrivacyTrix.Application;

internal static class DependencyInjection
{
    public static ServiceCollectionBuilder AddDataPrivacyTrixApplication(this ServiceCollectionBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.Services
            .AddCqrsMediator(assembly)
            .AddAxisValidator(assembly);

        return builder
            .AddCellphonesModule();
    }
}
