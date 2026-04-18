using AxisValidator;
using TenantTrix.Contracts.Tenants.v1.AddTenant;

namespace TenantTrix.Application.Tenants.UseCases.AddTenant.v1;

internal class AddTenantValidator : AxisValidatorBase<AddTenantCommand>
{
    public AddTenantValidator()
    {
        RequiredWithMaxLength(x => x.TenantName, "TENANT_NAME_NULL_OR_TOO_LONG");
    }
}
