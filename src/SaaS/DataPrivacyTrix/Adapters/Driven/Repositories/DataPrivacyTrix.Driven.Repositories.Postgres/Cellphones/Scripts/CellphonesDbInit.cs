namespace DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones.Scripts;

public static class CellphonesDbInit
{
    public const string Schema = "DATA_PRIVACY_TRIX_CELLPHONES";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", CellphonesTable.V1),
    ];
}
