namespace TenantTrix.Driven.Repositories.Postgres.Tenants.Scripts;

public static class TenantsDbInit
{
    public const string Schema = "TENANTS";

    public const string SeedAdminTenantId = "00000000-0000-7000-8000-000000000001";
    public const string SeedAdminTenantName = "admin-tenant";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", V1),
        ("V2", V2),
    ];

    private const string V1 = TenantsTable.V1;

    private static string V2 =>
        $"""
         INSERT INTO {TenantsTable.Table} ({TenantsTable.TenantId}, {TenantsTable.TenantName})
         SELECT '{SeedAdminTenantId}', '{SeedAdminTenantName}'
         WHERE NOT EXISTS (SELECT 1 FROM {TenantsTable.Table} WHERE {TenantsTable.TenantId} = '{SeedAdminTenantId}');
         """;
}
