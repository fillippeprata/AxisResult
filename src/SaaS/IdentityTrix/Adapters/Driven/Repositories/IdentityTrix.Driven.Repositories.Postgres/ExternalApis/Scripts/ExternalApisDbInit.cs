namespace IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

public static class ExternalApisDbInit
{
    public const string Schema = "IDENTITYTRIX_EXTERNAL_APIS";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", V1),
    ];

    private const string V1 = ExternalApisTable.V1;
}
