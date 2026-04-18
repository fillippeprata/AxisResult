using TenantTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;
using TenantTrix.Driven.Repositories.Postgres.Tenants.Scripts;

namespace TenantTrix.Driven.Repositories.Postgres;

public static class TenantTrixMigrations
{
    public static async Task InitializePostgresAsync(string connectionString)
    {
        await TenantsMigrations.InitializePostgresAsync(connectionString);
        await ExternalApisMigrations.InitializePostgresAsync(connectionString);
    }
}
