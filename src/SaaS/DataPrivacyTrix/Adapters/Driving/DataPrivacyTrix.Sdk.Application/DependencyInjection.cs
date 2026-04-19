using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Application;
using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Sdk.Application.Cellphones.v1;
using DataPrivacyTrix.Sdk.Application.Emails.v1;
using DataPrivacyTrix.Sdk.Application.Registration.v1;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Sdk.Application;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddDataPrivacyTrixSdkApplication(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<ICellphonesMediator, CellphonesMediator>();
        builder.Services.AddScoped<IEmailsMediator, EmailsMediator>();
        builder.Services.AddScoped<IRegistrationMediator, RegistrationMediator>();
        return builder.AddDataPrivacyTrixApplication();
    }
}
