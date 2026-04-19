namespace DataPrivacyTrix.Driven.Repositories.Postgres.Registration.Scripts;

public static class RegistrationDbInit
{
    public const string Schema = "DATA_PRIVACY_TRIX_REGISTRATION";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", AxisIdentitiesTable.V1),
        ("V2", AxisIdentityCellphonesTable.V1),
        ("V3", AxisIdentityEmailsTable.V1),
    ];
}
