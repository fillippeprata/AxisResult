using IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

namespace IdentityTrix.Driven.Repositories.Postgres;

public static class IdentityTrixMigrations
{
    public static Task InitializePostgresAsync(string connectionString)
        => ExternalApisMigrations.InitializePostgresAsync(connectionString);
}
