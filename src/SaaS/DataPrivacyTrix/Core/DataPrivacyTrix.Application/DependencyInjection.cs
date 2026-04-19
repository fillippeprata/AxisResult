using System.Reflection;
using AxisMediator.CQRS;
using AxisTrix.DependencyInjection;
using AxisValidator;
using DataPrivacyTrix.Application.Cellphones;
using DataPrivacyTrix.Application.Emails;
using DataPrivacyTrix.Application.Registration;

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
            .AddCellphonesModule()
            .AddEmailsModule()
            .AddRegistrationModule();
    }
}
