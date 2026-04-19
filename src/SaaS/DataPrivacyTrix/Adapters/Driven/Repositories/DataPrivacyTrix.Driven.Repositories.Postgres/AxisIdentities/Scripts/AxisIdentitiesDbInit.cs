namespace DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities.Scripts;

public static class AxisIdentitiesDbInit
{
    public const string Schema = "AXIS_IDENTITIES";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", AxisIdentitiesTable.V1),
        ("V2", AxisIdentityCellphonesTable.V1),
        ("V3", AxisIdentityEmailsTable.V1),
    ];
}
