namespace IdentityTrix.Driven.Repositories.Postgres.ExternalApis.Scripts;

public static class ExternalApisTable
{
    public const string Table = $"{ExternalApisDbInit.Schema}.EXTERNAL_APIS";
    public const string ExternalApiId = "EXTERNAL_API_ID";
    public const string Name = "NAME";
    public const string Secret = "HASHED_SECRET";
    public const string V1 = $"""
                              CREATE TABLE IF NOT EXISTS {Table}
                              (
                                  {ExternalApiId} VARCHAR(250) PRIMARY KEY,
                                  {Name} VARCHAR(250) NOT NULL UNIQUE,
                                  {Secret} VARCHAR(512) NOT NULL
                              );
                          """;
}
