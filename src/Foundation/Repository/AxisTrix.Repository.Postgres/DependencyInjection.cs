using AxisTrix.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Repository.Postgres;

public static class DependencyInjection
{
    public static void AddPostgresUnitOfWork(this IServiceCollection services, string serviceKey, string connectionString)
    {
        services.AddNpgsqlDataSource(connectionString,
            connectionLifetime: ServiceLifetime.Scoped,
            dataSourceLifetime: ServiceLifetime.Singleton,
            serviceKey: serviceKey);

        services.AddKeyedScoped<PostgresUnitOfWorkProvider>(serviceKey);
        services.AddKeyedScoped<IAxisUnitOfWork>(serviceKey, (sp, key) => sp.GetRequiredKeyedService<PostgresUnitOfWorkProvider>(key).GetUnitOfWork(sp, key));
        services.AddKeyedScoped<IPostgresUnitOfWork>(serviceKey, (sp, key) => sp.GetRequiredKeyedService<PostgresUnitOfWorkProvider>(key).GetUnitOfWork(sp, key));
    }
}
