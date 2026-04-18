using Axis;
using Npgsql;
using TenantTrix.SharedKernel.ExternalApis;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Driven.Repositories.Postgres.ExternalApis;

internal record ExternalApiDbEntity(ExternalApiId ExternalApiId, string ApiName, string HashedSecret, TenantId TenantId) : IExternalApiEntityProperties
{
    internal static ExternalApiDbEntity FromReader(NpgsqlDataReader reader)
        => new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
}
