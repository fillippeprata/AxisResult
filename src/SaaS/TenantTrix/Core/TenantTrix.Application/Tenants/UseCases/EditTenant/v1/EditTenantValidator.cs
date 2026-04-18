using Axis;
using AxisValidator.FluentValidation;
using TenantTrix.Contracts.Tenants.v1.EditTenant;

namespace TenantTrix.Application.Tenants.UseCases.EditTenant.v1;

internal class EditTenantValidator : AxisValidatorBase<EditTenantCommand>
{
    public EditTenantValidator()
    {
        RequiredTryParse(x => x.TenantId, "TENANT_ID_NULL_OR_NOT_GUID_7", x => TenantId.TryParse(x, out _));
        RequiredWithMaxLength(x => x.TenantName, "TENANT_NAME_NULL_OR_TOO_LONG");
    }
}
