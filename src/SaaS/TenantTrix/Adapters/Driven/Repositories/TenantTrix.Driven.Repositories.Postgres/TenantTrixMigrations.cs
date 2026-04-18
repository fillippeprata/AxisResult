using TenantTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

namespace TenantTrix.Driven.Repositories.Postgres;

public static class TenantTrixMigrations
{
    public static Task InitializePostgresAsync(string connectionString)
        => ExternalApisMigrations.InitializePostgresAsync(connectionString);
}
