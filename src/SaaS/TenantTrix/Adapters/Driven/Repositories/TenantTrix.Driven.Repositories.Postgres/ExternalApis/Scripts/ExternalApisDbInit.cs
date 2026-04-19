using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

public static class ExternalApisDbInit
{
    public const string Schema = "EXTERNAL_APIS";

    public const string SeedAdminExternalApiId = "00000000-0000-7000-8000-000000000001";
    public const string SeedAdminApiName = "admin-external-api";
    public const string SeedAdminPlainSecret = "admin-seed-secret-replace-after-init";
    public const string SeedAdminTenantId = "00000000-0000-7000-8000-000000000001";
    public static readonly string SeedAdminHashedSecret = ExternalApiSecret.Hash(SeedAdminPlainSecret);

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", V1),
        ("V2", V2),
        ("V3", V3),
    ];

    private const string V1 = ExternalApisTable.V1;

    private static string V2 =>
        $"""
         INSERT INTO {ExternalApisTable.Table} ({ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret})
         SELECT '{SeedAdminExternalApiId}', '{SeedAdminApiName}', '{SeedAdminHashedSecret}'
         WHERE NOT EXISTS (SELECT 1 FROM {ExternalApisTable.Table} WHERE {ExternalApisTable.ExternalApiId} = '{SeedAdminExternalApiId}');
         """;

    private const string V3 = ExternalApisTable.V3;
}
