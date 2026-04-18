using Axis;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Contracts.Tenants.v1.EditTenant;

namespace TenantTrix.Contracts.Tenants.v1;

public interface ITenantsMediator
{
    Task<AxisResult<AddTenantResponse>> AddAsync(AddTenantCommand command);
    Task<AxisResult<EditTenantResponse>> EditAsync(EditTenantCommand command);
}
