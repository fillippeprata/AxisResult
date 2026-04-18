using Axis;
using Npgsql;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Driven.Repositories.Postgres.Tenants;

internal record TenantDbEntity(TenantId TenantId, string TenantName) : ITenantEntityProperties
{
    internal static TenantDbEntity FromReader(NpgsqlDataReader reader)
        => new(reader.GetString(0), reader.GetString(1));
}
