namespace DataPrivacyTrix.Driven.Repositories.Postgres.Emails.Scripts;

public static class EmailsDbInit
{
    public const string Schema = "EMAILS";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", EmailsTable.V1),
    ];
}
