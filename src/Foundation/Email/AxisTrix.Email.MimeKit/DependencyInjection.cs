using AxisTrix.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Email.MimeKit;

public static class EmailDependencyInjection
{
    public static ServiceCollectionBuilder AddEmailTrix(this ServiceCollectionBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<AxisEmailSettings>(configuration.GetSection("EmailTrixSettings"));
        builder.Services.AddScoped<IAxisEmailService, AxisEmailService>();
        return builder;
    }
}
