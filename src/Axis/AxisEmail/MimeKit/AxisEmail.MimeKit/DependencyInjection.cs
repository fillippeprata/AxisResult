using Axis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AxisEmail.MimeKit;

public static class EmailDependencyInjection
{
    public static IServiceCollection AddEmailTrix(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AxisEmailSettings>(configuration.GetSection("EmailTrixSettings"));
        services.AddScoped<IAxisEmailService, AxisEmailService>();
        return services;
    }
}
