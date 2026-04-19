namespace DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities.Scripts;

public static class AxisIdentityEmailsTable
{
    public const string Table = $"{AxisIdentitiesDbInit.Schema}.AXIS_IDENTITY_EMAILS";
    public const string AxisIdentityId = "AXIS_IDENTITY_ID";
    public const string EmailId = "EMAIL_ID";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {AxisIdentityId} VARCHAR(250) NOT NULL,
                                  {EmailId} VARCHAR(250) NOT NULL,
                                  PRIMARY KEY ({AxisIdentityId}, {EmailId}),
                                  CONSTRAINT UQ_AXIS_IDENTITY_EMAILS_EMAIL UNIQUE ({EmailId}),
                                  CONSTRAINT FK_AXIS_IDENTITY_EMAILS_IDENTITY
                                      FOREIGN KEY ({AxisIdentityId})
                                      REFERENCES {AxisIdentitiesTable.Table} ({AxisIdentitiesTable.AxisIdentityId})
                                      ON DELETE CASCADE
                              );
                          """;
}
