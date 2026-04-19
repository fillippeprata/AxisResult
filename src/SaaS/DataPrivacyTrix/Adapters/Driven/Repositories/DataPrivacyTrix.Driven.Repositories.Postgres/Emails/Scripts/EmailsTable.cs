namespace DataPrivacyTrix.Driven.Repositories.Postgres.Emails.Scripts;

public static class EmailsTable
{
    public const string Table = $"{EmailsDbInit.Schema}.EMAILS";
    public const string EmailId = "EMAIL_ID";
    public const string EmailAddress = "EMAIL_ADDRESS";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {EmailId} VARCHAR(250) PRIMARY KEY,
                                  {EmailAddress} VARCHAR(320) NOT NULL UNIQUE
                              );
                          """;
}
