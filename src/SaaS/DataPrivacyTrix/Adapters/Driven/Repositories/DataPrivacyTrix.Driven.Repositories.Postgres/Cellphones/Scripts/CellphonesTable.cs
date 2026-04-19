namespace DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones.Scripts;

public static class CellphonesTable
{
    public const string Table = $"{CellphonesDbInit.Schema}.CELLPHONES";
    public const string CellphoneId = "CELLPHONE_ID";
    public const string CountryId = "COUNTRY_ID";
    public const string CellphoneNumber = "CELLPHONE_NUMBER";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {CellphoneId} VARCHAR(250) PRIMARY KEY,
                                  {CountryId} VARCHAR(5) NOT NULL,
                                  {CellphoneNumber} VARCHAR(30) NOT NULL,
                                  CONSTRAINT UQ_CELLPHONES_COUNTRY_NUMBER UNIQUE ({CountryId}, {CellphoneNumber})
                              );
                          """;
}
