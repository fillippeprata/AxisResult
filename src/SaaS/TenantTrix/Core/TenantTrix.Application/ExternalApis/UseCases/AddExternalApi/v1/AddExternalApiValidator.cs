using AxisValidator.FluentValidation;
using TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;

namespace TenantTrix.Application.ExternalApis.UseCases.AddExternalApi.v1;

internal class AddExternalApiValidator : AxisValidatorBase<AddExternalApiCommand>
{
    public AddExternalApiValidator()
    {
        RequiredWithMaxLength(x => x.ApiName, "EXTERNAL_API_NAME_NULL_OR_TOO_LONG");
        RequiredGuid7(x => x.TenantId, "EXTERNAL_API_TENANT_ID_NULL_OR_NOT_GUID_7");
    }
}
