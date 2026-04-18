namespace TenantTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

public static class ExternalApisTable
{
    public const string Table = $"{ExternalApisDbInit.Schema}.EXTERNAL_APIS";
    public const string ExternalApiId = "EXTERNAL_API_ID";
    public const string Name = "NAME";
    public const string Secret = "HASHED_SECRET";
    public const string TenantId = "TENANT_ID";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {ExternalApiId} VARCHAR(250) PRIMARY KEY,
                                  {Name} VARCHAR(250) NOT NULL UNIQUE,
                                  {Secret} VARCHAR(512) NOT NULL
                              );
                          """;

    public const string V3 = $"""
                              ALTER TABLE {Table} ADD COLUMN IF NOT EXISTS {TenantId} VARCHAR(250);
                              UPDATE {Table} SET {TenantId} = '{ExternalApisDbInit.SeedAdminTenantId}' WHERE {TenantId} IS NULL;
                              ALTER TABLE {Table} ALTER COLUMN {TenantId} SET NOT NULL;
                          """;
}
