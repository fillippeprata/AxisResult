using Axis;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Domain.Tenants.Root;

internal class TenantEntity(
    TenantId tenantId,
    string tenantName)
    : ITenantEntityProperties
{
    #region Properties

    public TenantId TenantId { get; } = tenantId;
    public string TenantName { get; } = tenantName;

    internal TenantEntity(ITenantEntityProperties properties) : this(
        properties.TenantId,
        properties.TenantName
    ) {}

    #endregion
}
