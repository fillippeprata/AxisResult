using Axis;

namespace TenantTrix.SharedKernel.Tenants;

public interface ITenantEntityProperties
{
    TenantId  TenantId { get; }
    string TenantName { get; }
}
