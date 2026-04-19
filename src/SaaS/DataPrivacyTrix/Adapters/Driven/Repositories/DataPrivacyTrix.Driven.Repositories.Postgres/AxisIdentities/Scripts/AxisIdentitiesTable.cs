namespace DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities.Scripts;

public static class AxisIdentitiesTable
{
    public const string Table = $"{AxisIdentitiesDbInit.Schema}.AXIS_IDENTITIES";
    public const string AxisIdentityId = "AXIS_IDENTITY_ID";
    public const string IsIndividual = "IS_INDIVIDUAL";
    public const string Document = "DOCUMENT";
    public const string CountryId = "COUNTRY_ID";
    public const string DisplayName = "DISPLAY_NAME";
    public const string DefaultLanguage = "DEFAULT_LANGUAGE";
    public const string SecurityLevel = "SECURITY_LEVEL";

    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {AxisIdentityId} VARCHAR(250) PRIMARY KEY,
                                  {IsIndividual} BOOLEAN NOT NULL,
                                  {Document} VARCHAR(20) NOT NULL,
                                  {CountryId} VARCHAR(5) NOT NULL,
                                  {DisplayName} VARCHAR(255) NOT NULL,
                                  {DefaultLanguage} VARCHAR(20) NOT NULL,
                                  {SecurityLevel} VARCHAR(20) NOT NULL,
                                  CONSTRAINT UQ_AXIS_IDENTITIES_COUNTRY_DOCUMENT UNIQUE ({CountryId}, {Document})
                              );
                          """;
}
