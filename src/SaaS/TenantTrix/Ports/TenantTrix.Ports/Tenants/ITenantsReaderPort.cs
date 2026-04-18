using Axis;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Ports.Tenants;

public interface ITenantsReaderPort
{
    Task<AxisResult<ITenantEntityProperties>> GetByIdAsync(TenantId id);
    Task<AxisResult<ITenantEntityProperties>> GetByNameAsync(string tenantName);
}
