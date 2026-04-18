namespace TenantTrix.Driven.Repositories.Postgres.Tenants.Scripts;

public static class TenantsTable
{
    public const string Table = $"{TenantsDbInit.Schema}.TENANTS";
    public const string TenantId = "TENANT_ID";
    public const string TenantName = "TENANT_NAME";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {TenantId} VARCHAR(250) PRIMARY KEY,
                                  {TenantName} VARCHAR(250) NOT NULL UNIQUE
                              );
                          """;
}
