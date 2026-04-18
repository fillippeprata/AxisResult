using Axis;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Ports.Tenants;

public interface ITenantsWritePort
{
    Task<AxisResult> CreateAsync(ITenantEntityProperties properties);
    Task<AxisResult> UpdateNameAsync(TenantId id, string tenantName);
}
