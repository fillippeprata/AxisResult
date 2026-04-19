namespace DataPrivacyTrix.Driven.Repositories.Postgres.Registration.Scripts;

public static class AxisIdentityCellphonesTable
{
    public const string Table = $"{RegistrationDbInit.Schema}.AXIS_IDENTITY_CELLPHONES";
    public const string AxisIdentityId = "AXIS_IDENTITY_ID";
    public const string CellphoneId = "CELLPHONE_ID";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {AxisIdentityId} VARCHAR(250) NOT NULL,
                                  {CellphoneId} VARCHAR(250) NOT NULL,
                                  PRIMARY KEY ({AxisIdentityId}, {CellphoneId}),
                                  CONSTRAINT UQ_AXIS_IDENTITY_CELLPHONES_CELLPHONE UNIQUE ({CellphoneId}),
                                  CONSTRAINT FK_AXIS_IDENTITY_CELLPHONES_IDENTITY
                                      FOREIGN KEY ({AxisIdentityId})
                                      REFERENCES {AxisIdentitiesTable.Table} ({AxisIdentitiesTable.AxisIdentityId})
                                      ON DELETE CASCADE
                              );
                          """;
}
