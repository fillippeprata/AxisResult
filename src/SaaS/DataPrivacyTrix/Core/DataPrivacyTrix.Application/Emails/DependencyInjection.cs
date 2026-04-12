using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.Application.Emails;

internal static class DependencyInjection
{
    internal static ServiceCollectionBuilder AddEmailsModule(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IEmailAggregateApplicationFactory, EmailAggregateApplicationFactory>();
        return builder;
    }
}
