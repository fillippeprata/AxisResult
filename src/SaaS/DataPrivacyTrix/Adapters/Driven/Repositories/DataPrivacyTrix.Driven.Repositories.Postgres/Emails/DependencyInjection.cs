using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Ports.Emails;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Emails;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddEmailsRepository(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<EmailsRepository>();
        builder.Services.AddScoped<IEmailsReaderPort>(sp => sp.GetRequiredService<EmailsRepository>());
        builder.Services.AddScoped<IEmailsWritePort>(sp => sp.GetRequiredService<EmailsRepository>());
        return builder;
    }
}
